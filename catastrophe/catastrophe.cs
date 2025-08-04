using System.Reflection;
using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Timers;
using Epic.OnlineServices;
using System.IO;
using JetBrains.Annotations;

namespace catastrophe
{
    public class catastrophe : ModBehaviour
    {
        public static catastrophe Instance;
        //Copied from the timers module example, but I can actually understand this with my basic grasp of programming.
        //- private static System.Timers.Timer theTimer;
        public INewHorizons NewHorizons;

        public void Awake()
        {
            Instance = this;
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
        }

        public void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"My mod {nameof(catastrophe)} is loaded!", MessageType.Success);

            // Get the New Horizons API and load configs
            NewHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            NewHorizons.LoadConfigs(this);
            //I might be a dumbass who forgot this line.
            //Legendary mistake.
            //It would've definitely helped if I remembered listeners existed (me who was using listeners in JS earlier)
            NewHorizons.GetChangeStarSystemEvent().AddListener(PreSystemLoad);

            new Harmony("Onbvb.catastrophe").PatchAll(Assembly.GetExecutingAssembly());
            MakeThatStar(); //Forced to call this once in start because titlescreen to system doesnt count for getchangestarsystemevent. This prevents the first loop from lacking a star.
        }

        public void MakeThatStar()
        {
            ModHelper.Console.WriteLine("Making Star.", MessageType.Info);
            var gottaGoFast = ModHelper.Config.GetSettingsValue<bool>("Speedrunner Mode"); //Check for speedrunners
            ModHelper.Console.WriteLine("Speedrunner Mode is set to: " + gottaGoFast, MessageType.Info);
            //    Get the file as a string for creating the star.
            StreamReader speedStar = new StreamReader(ModHelper.Manifest.ModFolderPath + "planets/justInTime/catStarSpeedrun.txt");
            StreamReader slowStar = new StreamReader(ModHelper.Manifest.ModFolderPath + "planets/justInTime/catStar.txt");
            //    make the star's life significantly shorter if they're speedrunning. (lore reason is that the nomai are doing their experiment NOW)
            if (gottaGoFast == true) NewHorizons.CreatePlanet(config: speedStar.ReadLine(), this);
            else NewHorizons.CreatePlanet(config: slowStar.ReadLine(), this);
            ModHelper.Console.WriteLine("MakeThatStar has ended.", MessageType.Info);
        }

        //This is code for the dialogue change feature.
        //Unused because thats happening next patch.
        /*
        private static void TurquoiseTimer()
        {
            //TODO: find a method of removing any timers that might linger.
            // Or, use TimeLoop.GetSecondsElapsed.
            //Set the condition to false (just in case)
            DialogueConditionManager.SharedInstance.SetConditionState("amethystIsFuckingDead", false);
            //Timer. When it ends it runs the next thing (i could probably have it just run the single line itself.)
            theTimer = new System.Timers.Timer(72000);
            theTimer.Elapsed += TurquoiseSadness;
            theTimer.AutoReset = false;
            theTimer.Enabled = true;
        }

        private static void TurquoiseSadness(Object source, ElapsedEventArgs e)
        {
            //When the timer ends, set 'amethystIsFuckingDead' to true (because amethyst is fucking dead)
            DialogueConditionManager.SharedInstance.SetConditionState("amethystIsFuckingDead", true);
        } */
        public void PreSystemLoad(string system)
        {
            if (system != "Jam5") //Let's try this...
            {
                return;
            }
            MakeThatStar();
        }

        //TODO: Find a way of listening for the loop's start, then create a timer that lasts 12 minutes and sets the turquoise condition.
        // TODO: Find a way of listening for player waking up (and time starting to flow).
        //  Clock mod uses GlobalMessenger.AddListener("WakeUp", OnWakeUp); - OnWakeUp points to a function. This is probably exactly what I'm looking for.
        // Maybe instead of a timer use TimeLoop.GetSecondsElapsed()?
        // Hell there might be an event for the star's supernova happening. Could check for that (since speedrunner mode would change the time it takes.)

        //Attempted methods for doing speedrunner mode:
        // Create the star using code (star generates on first loop only)
        // Create the star using code after the system loads (failed)
        // Create the entire system using code (failed)
        // Have the system orbit a dummy object and create the star using code (star generates on second loop forward, orbits fail due to dummy object's lack of mass.)
        //Ideas
        // Dummy object, create entire system based on that object (star is static on it, everything else orbits star) - would probably end up breaking in the first loop, like attempt 4
        // Uhhhhhhhh. Cry. and/or remove this feature idea.
        // Have the system orbit catastrophe and make the star on runtime.
        //... have i tried not forgetting to add a listener?
        // I swear to everything holy if that was the issue im going to crash out.
        // This code is ass. Session Terminated.
    }

}
