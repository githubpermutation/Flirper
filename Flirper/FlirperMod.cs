using System;
using ICities;

namespace Flirper
{
    public class FlirperMod : IUserMod
    {
        public string Name {
            get {
                FlirperBootstrap.flirpIt ();
                return "Flirper (beta)";
            }
        }

        public string Description {
            get {
                return "Randomly changes the main menu background image";
            }
        }
    }
}