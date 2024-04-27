using Terraria;
using Terraria.ModLoader;
using System;
using System.Linq;

using Spooky.Content.UserInterfaces;

namespace Spooky.Core
{
    //separate modplayer for all of the bloom buffs since there is a LOT of them
    public class BloomBuffsPlayer : ModPlayer
    {
        //list of strings for each buff slot
        public string[] BloomBuffSlots = new string[4];

        //durations for each buff slot
        public int Duration1 = 0;
        public int Duration2 = 0;
        public int Duration3 = 0;
        public int Duration4 = 0;

        //bools for each edible bloom
        public bool SunHappyFlower = false;
        public bool SunLemon = false;
        public bool SunOrange = false;
        public bool SunPineapple = false;

        //when the player consumes a fruit, add that fruits name to the buff list and set its duration
        public void AddBuffToList(string BuffName, int Duration)
        {
            //if the buff itself exists in the list already, then reset the duration similarly to how vanilla potions reset the duration if you have the buff already
            //then return so that the buffs name doesnt get added to the list again
            if (BloomBuffSlots.Contains(BuffName))
            {
                if (BloomBuffSlots[0] == BuffName)
                {
                    Duration1 = Duration;
                }
                else if (BloomBuffSlots[1] == BuffName)
                {
                    Duration2 = Duration;
                }
                else if (BloomBuffSlots[2] == BuffName)
                {
                    Duration3 = Duration;
                }

                return;
            }

            //add the buff to the list by checking each slot to see if its open, and if it is add that buff to that slot
            if (BloomBuffSlots[0] == string.Empty)
            {
                BloomBuffSlots[0] = BuffName;
                Duration1 = Duration;
            }
            else if (BloomBuffSlots[1] == string.Empty)
            {
                BloomBuffSlots[1] = BuffName;
                Duration2 = Duration;
            }
            else if (BloomBuffSlots[2] == string.Empty)
            {
                BloomBuffSlots[2] = BuffName;
                Duration3 = Duration;
            }
        }

        //manually set the bools for each bonus if the list of buffs contains that buff name
        //since the buffs order doesnt matter, just checking the slot list if it contains that respective string is probably fine
        public void GivePlayerBloomBonus()
        {
            if (BloomBuffSlots.Contains("SunHappyFlower"))
            {
                SunHappyFlower = true;
            }
            if (BloomBuffSlots.Contains("SunLemon"))
            {
                SunLemon = true;
            }
            if (BloomBuffSlots.Contains("SunOrange"))
            {
                SunOrange = true;
            }
            if (BloomBuffSlots.Contains("SunPineapple"))
            {
                SunPineapple = true;
            }
        }

        //handler for the buffs duration decreasing over time and setting each buff slot back to blank if the duration of that buff slot runs out
        public void HandleBloomBuffDuration()
        {
            //open the bloom buff UI if you have a buff, if not then close it
            if (BloomBuffSlots[0] == string.Empty && BloomBuffSlots[1] == string.Empty && BloomBuffSlots[2] == string.Empty && BloomBuffSlots[3] == string.Empty)
            {
                BloomBuffUIBox.UIOpen = false;
            }
            else
            {
                BloomBuffUIBox.UIOpen = true;
            }

            if (Duration1 > 0)
            {
                Duration1--;
            }
            else
            {
                BloomBuffSlots[0] = string.Empty;
            }

            if (Duration2 > 0)
            {
                Duration2--;
            }
            else
            {
                BloomBuffSlots[1] = string.Empty;
            }

            if (Duration3 > 0)
            {
                Duration3--;
            }
            else
            {
                BloomBuffSlots[2] = string.Empty;
            }

            if (Duration4 > 0)
            {
                Duration4--;
            }
            else
            {
                BloomBuffSlots[3] = string.Empty;
            }
        }

        public override void ResetEffects()
        {
            SunHappyFlower = false;
            SunLemon = false;
            SunOrange = false;
            SunPineapple = false;
        }

        public override void PreUpdate()
        { 
            HandleBloomBuffDuration();
            GivePlayerBloomBonus();
        }
    }
}