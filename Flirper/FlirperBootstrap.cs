using System;
using ICities;
using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework.HTTP;

namespace Flirper
{
    public class FlirperBootstrap : MonoBehaviour
    {
        public static void flirpIt ()
        {
            if (UIView.GetAView ().FindUIComponent ("FlirperAttribution") != null)
                return;

            UIComponent background = UIView.GetAView ().FindUIComponent ("BackgroundSprite");
            if (background == null)
                return;
                
            UITextureSprite bgsprite = background.GetComponent<UITextureSprite> ();
            if (bgsprite == null)
                return;

            ImageListEntry entry = ImageList.getRandomEntry ();
            if (entry == null) {
                DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Error, "Flirper: could not get an image entry. Check format of FlirperImageList.txt");
                return;
            }

            try {
                changeBackgroundImage (bgsprite, entry);
            } catch (Exception ex) {
                DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Error, "Flirper: " + ex.ToString ());
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
                    assignImageData (bgsprite, imgdata);
                    createLabel (entry);
                }
            };
        }

        static void changeBackgroundImage (UITextureSprite bgsprite, ImageListEntry entry)
        {
            if (entry.isHTTP) {
                Request imgget = new Request ("get", entry.uri);
                imgget.AddHeader ("Accept-Language", "en");

                imgget.Send(httpCallback(bgsprite, entry));
            } else {
                // read path @verbatim to avoid escaping
                byte[] imgdata = System.IO.File.ReadAllBytes (@entry.uri);
                assignImageData (bgsprite, imgdata);
                createLabel(entry);
            }
        }

        static void createLabel (ImageListEntry entry)
        {
            UILabel flirperAttribution;

            if (UIView.GetAView ().FindUIComponent ("FlirperAttribution") == null) {
                flirperAttribution = UIView.GetAView ().AddUIComponent (typeof(UILabel)) as UILabel;
                flirperAttribution.name = "FlirperAttribution";
            } else {
                flirperAttribution = UIView.GetAView ().FindUIComponent ("FlirperAttribution") as UILabel;
            }

            flirperAttribution.textAlignment = UIHorizontalAlignment.Right;
            flirperAttribution.textScale = 1.5f;

            flirperAttribution.text = "";

            if (!String.IsNullOrEmpty (entry.title)) {
                flirperAttribution.text += entry.title;
            }

            if (!String.IsNullOrEmpty (entry.author)) {
                flirperAttribution.text += " (by " + entry.author + ")";
            }

            if (!String.IsNullOrEmpty (entry.source)) {
                flirperAttribution.text += "\n" + entry.source;
            }

            flirperAttribution.outlineColor = new Color32 (0, 0, 0, 200);
            flirperAttribution.outlineSize = 1;
            flirperAttribution.useOutline = true;

            flirperAttribution.pivot = UIPivotPoint.TopRight;
            flirperAttribution.transformPosition = UIView.GetAView ().GetBounds ().max;
            flirperAttribution.relativePosition += new Vector3 (-10, 10);
        }

        static void assignImageData (UITextureSprite bgsprite, byte[] imgdata)
        {
            Texture2D bg = new Texture2D (1, 1);
            bg.LoadImage (imgdata);

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