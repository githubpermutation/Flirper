using System;
using ICities;
using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework.HTTP;

namespace Flirper
{
    public class FlirperBootstrap : MonoBehaviour
    {
        private static readonly string ModTag = "[Flirper]";

        public static void flirpIt ()
        {
            UIComponent background = UIView.GetAView ().FindUIComponent ("BackgroundSprite");
            if (background == null)
                return;
                
            UITextureSprite bgsprite = background.GetComponent<UITextureSprite> ();
            if (bgsprite == null)
                return;

            initLabel (); 

            ImageListEntry entry = ImageList.getRandomEntry ();
            if (entry == null) {
                DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Error, FlirperBootstrap.ModTag+" could not get an image entry. Check format of FlirperImageList.txt");
                return;
            }

            try {
                changeBackgroundImage (bgsprite, entry);
            } catch (Exception ex) {
                DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Error, FlirperBootstrap.ModTag+" " + ex.ToString ());
            }                
        }

        static Action<Request> httpCallback (UITextureSprite bgsprite, ImageListEntry entry)
        {
            return delegate (Request req) {
                if (req.isDone) {
                    if (req.exception != null) {
                        throw req.exception;
                    }
                    if (req.response == null || req.response.status != 200) {
                        throw new Exception ("error while fetching " + entry.uri);
                    }
                    byte[] imgdata = req.response.bytes;
                    Texture2D bg = new Texture2D (1, 1);
                    bg.LoadImage (imgdata);
                    
                    assignImageData (bgsprite, bg);
                    changeLabel (entry);
                }
            };
        }

        static void changeBackgroundImage (UITextureSprite bgsprite, ImageListEntry entry)
        {
            if (entry.isLatestSaveGame) {
                SaveGameMetaData latestSaveGame = SaveHelper.GetLatestSaveGame ();

                if (latestSaveGame == null) {
                    throw new Exception ("no savegame available");
                }

                String title = latestSaveGame.cityName;
                String extraInfo = latestSaveGame.timeStamp.ToShortDateString ();

                Texture2D savegameimage = latestSaveGame.imageRef.Instantiate<Texture> () as Texture2D;
                savegameimage = Blur.FastBlur (savegameimage, 2, 2);
                assignImageData (bgsprite, savegameimage);

                changeLabel (new ImageListEntry ("", title, "", extraInfo));
            } else if (entry.isHTTP) {
                Request imgget = new Request ("get", entry.uri);
                imgget.AddHeader ("Accept-Language", "en");

                imgget.Send (httpCallback (bgsprite, entry));
            } else {
                // read path @verbatim to avoid escaping
                byte[] imgdata = System.IO.File.ReadAllBytes (@entry.uri);

                Texture2D bg = new Texture2D (1, 1);
                bg.LoadImage (imgdata);

                assignImageData (bgsprite, bg);
                changeLabel(entry);
            }
        }

        static void initLabel ()
        {
            if (UIView.GetAView ().FindUIComponent ("FlirperAttribution") == null) {
                UILabel flirperAttribution = UIView.GetAView ().AddUIComponent (typeof(UILabel)) as UILabel;
                flirperAttribution.name = "FlirperAttribution";
                flirperAttribution.eventClick += loadNextFlirp;
            }
        }

        static void changeLabel (ImageListEntry entry)
        {
            UILabel flirperAttribution = UIView.GetAView ().FindUIComponent ("FlirperAttribution") as UILabel;

            flirperAttribution.textAlignment = UIHorizontalAlignment.Right;
            flirperAttribution.textScale = 2f;
            flirperAttribution.textScaleMode = UITextScaleMode.ScreenResolution;

            flirperAttribution.text = "";

            if (!String.IsNullOrEmpty (entry.title)) {
                flirperAttribution.text += entry.title;
            }

            if (!String.IsNullOrEmpty (entry.author)) {
                flirperAttribution.text += " (by " + entry.author + ")";
            }

            if (!String.IsNullOrEmpty (entry.extraInfo)) {
                flirperAttribution.text += "\n" + entry.extraInfo;
            }

            flirperAttribution.outlineColor = new Color32 (0, 0, 0, 200);
            flirperAttribution.outlineSize = 1;
            flirperAttribution.useOutline = true;

            flirperAttribution.relativePosition = new Vector3 (UIView.GetAView().GetScreenResolution().x - flirperAttribution.width, 0);
            flirperAttribution.relativePosition += new Vector3 (-10, 10);
        }

        static void loadNextFlirp (UIComponent component, UIMouseEventParameter eventParam)
        {
            UILabel label = ((UILabel)component);
            if(label.text != "Loading"){
                label.text = "Loading";
                FlirperBootstrap.flirpIt();
            }
        }

        static void assignImageData (UITextureSprite bgsprite, Texture2D bg)
        {
            //reset to allow multiple flirps without stretching
            bgsprite.width = UIView.GetAView ().GetScreenResolution ().x;
            bgsprite.height = UIView.GetAView ().GetScreenResolution ().y;

            float widthFactor = UIView.GetAView ().GetScreenResolution ().x / bg.width;
            float heightFactor = UIView.GetAView ().GetScreenResolution ().y / bg.height;

            //destretch
            bgsprite.width /= widthFactor;
            bgsprite.height /= heightFactor;

            //scale to fill screen
            float scaleFactor = Mathf.Max (widthFactor, heightFactor);
            bgsprite.width *= scaleFactor;
            bgsprite.height *= scaleFactor;

            bgsprite.texture = bg;
            bgsprite.absolutePosition = new Vector3 (0, 0);
        }
    }
 }