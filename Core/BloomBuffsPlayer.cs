using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

using Spooky.Content.UserInterfaces;

namespace Spooky.Core
{
    //separate modplayer for all of the bloom buffs since there is a ton of them and i dont feel like cluttering SpookyPlayer
    public class BloomBuffsPlayer : ModPlayer
    {
        //list of strings for each buff slot
        //each consumable bloom adds its name to a slot in this list of strings, and then each bonus is applied if that string is in the list
        //also used in the bloom UI so that it can draw each respective buff icons on it
        public string[] BloomBuffSlots = new string[4];

        //durations for each buff slot
        public int Duration1 = 0;
        public int Duration2 = 0;
        public int Duration3 = 0;
        public int Duration4 = 0;

        //bools for each edible bloom
        public bool WinterBlackberry = false;
        public bool WinterBlueberry = false;
        public bool WinterGooseberry = false;
        public bool WinterStrawberry = false;
        public bool SummerLemon = false;
        public bool SummerOrange = false;
        public bool SummerPineapple = false;
        public bool SummerSunflower = false;
        public bool Dragonfruit = false;

        //misc stuff
        public int DragonfruitStacks = 0;
        public bool UnlockedSlot3 = false;
        public bool UnlockedSlot4 = false;

        //UI default position
        public Vector2 UITopLeft = new Vector2(Main.screenWidth / 2 - 116f, 75f);

        //global bool used for each individual bloom item so that they cannot be eaten if all of your slots are filled
        public bool CanConsumeFruit(string BuffName)
        {
            //if the player eats a bloom they already have active, allow it to be used so it can reset the buff duration
            if (BloomBuffSlots.Contains(BuffName))
            {
                return true;
            }
            else
            {
                //if every single buff slot is filled and both slot 3 and slot 4 are locked, dont allow the player to consume a bloom
                if (BloomBuffSlots[0] != string.Empty && BloomBuffSlots[1] != string.Empty && !UnlockedSlot3 && !UnlockedSlot4)
                {
                    return false;
                }
                //if every single buff slot is filled and the fourth slot is locked, dont allow the player to consume a bloom
                if (BloomBuffSlots[0] != string.Empty && BloomBuffSlots[1] != string.Empty && BloomBuffSlots[2] != string.Empty && !UnlockedSlot4)
                {
                    return false;
                }
                //if every single buff slot is filled, dont allow the player to consume a bloom
                if (BloomBuffSlots[0] != string.Empty && BloomBuffSlots[1] != string.Empty && BloomBuffSlots[2] != string.Empty && BloomBuffSlots[3] != string.Empty)
                {
                    return false;
                }
            }

            return true;
        }

        //when the player consumes a bloom, add that blooms name to a buff list slot and set its duration in that specific slot
        public void AddBuffToList(string BuffName, int Duration)
        {
            //if the player consumes a bloom they already have, then reset the duration similarly to how drinking potions in terraria resets their duration if you have the buff already
            //then return so that buffs name doesnt get added to the list again
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
                else if (BloomBuffSlots[2] == BuffName && UnlockedSlot3)
                {
                    Duration3 = Duration;
                }
                else if (BloomBuffSlots[3] == BuffName && UnlockedSlot4)
                {
                    Duration3 = Duration;
                }

                return;
            }

            //add the buff to the list by checking each slot to see if its open, and if it is add that buff to that slot
            //only attempt to check beyond the second slot when the player has each unlockable slot unlocked
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
            else if (BloomBuffSlots[2] == string.Empty && UnlockedSlot3)
            {
                BloomBuffSlots[2] = BuffName;
                Duration3 = Duration;
            }
            else if (BloomBuffSlots[3] == string.Empty && UnlockedSlot4)
            {
                BloomBuffSlots[3] = BuffName;
                Duration3 = Duration;
            }
        }

        //manually set the bools for each bonus if the list of buffs contains that buff name
        //since the buffs order doesnt matter, just checking the slot list if it contains that respective string is fine
        //this probably doesnt look very pretty but whatever, cannot be bothered to change it right now
        public void GivePlayerBloomBonus()
        {
            if (BloomBuffSlots.Contains("WinterBlackberry"))
            {
                WinterBlackberry = true;
            }
            if (BloomBuffSlots.Contains("WinterBlueberry"))
            {
                WinterBlueberry = true;
            }
            if (BloomBuffSlots.Contains("WinterGooseberry"))
            {
                WinterGooseberry = true;
            }
            if (BloomBuffSlots.Contains("WinterStrawberry"))
            {
                WinterStrawberry = true;
            }
            if (BloomBuffSlots.Contains("SummerLemon"))
            {
                SummerLemon = true;
            }
            if (BloomBuffSlots.Contains("SummerOrange"))
            {
                SummerOrange = true;
            }
            if (BloomBuffSlots.Contains("SummerPineapple"))
            {
                SummerPineapple = true;
            }
            if (BloomBuffSlots.Contains("SummerSunflower"))
            {
                SummerSunflower = true;
            }
            if (BloomBuffSlots.Contains("Dragonfruit"))
            {
                Dragonfruit = true;
            }
        }

        //handler for the buffs duration decreasing over time and setting each buff slot back to blank if the duration of that buff slot runs out
        public void HandleBloomBuffDuration()
        {
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

            //automatically set the string to empty if the player doesnt have the additional 3rd slot unlocked
            if (Duration3 > 0 && UnlockedSlot3)
            {
                Duration3--;
            }
            else
            {
                BloomBuffSlots[2] = string.Empty;
            }

            //automatically set the string to empty if the player doesnt have the additional 4th slot unlocked
            if (Duration4 > 0 && UnlockedSlot4)
            {
                Duration4--;
            }
            else
            {
                BloomBuffSlots[3] = string.Empty;
            }
        }

        //save and load the unlocked slots so they are permanent
        public override void SaveData(TagCompound tag)
        {
            tag["UITopLeft"] = UITopLeft;

            if (UnlockedSlot3) tag["UnlockedSlot3"] = true;
            if (UnlockedSlot4) tag["UnlockedSlot4"] = true;
        }
        public override void LoadData(TagCompound tag)
        {
            UITopLeft = tag.Get<Vector2>("UITopLeft");

            UnlockedSlot3 = tag.ContainsKey("UnlockedSlot3");
            UnlockedSlot4 = tag.ContainsKey("UnlockedSlot4");
        }

        public override void ResetEffects()
        {
            WinterBlackberry = false;
            WinterBlueberry = false;
            WinterGooseberry = false;
            WinterStrawberry = false;
            SummerLemon = false;
            SummerOrange = false;
            SummerPineapple = false;
            SummerSunflower = false;
            Dragonfruit = false;
        }

        public override void PreUpdate()
        {
            //open the bloom buff UI if you have any bloom buff at all, if not then close it
            //instead of just appearing, make the UI fade in for a cool effect if the player eats a bloom
            if (BloomBuffSlots[0] == string.Empty && BloomBuffSlots[1] == string.Empty && BloomBuffSlots[2] == string.Empty && BloomBuffSlots[3] == string.Empty)
            {
                if (BloomBuffUI.Transparency > 0f)
                {
                    BloomBuffUI.Transparency -= 0.05f;
                }
            }
            else
            {
                if (!Main.playerInventory)
                { 
                    if (BloomBuffUI.Transparency < 1f)
                    {
                        BloomBuffUI.Transparency += 0.05f;
                    }
                }
                //fade out a little if the players inventory is open
                else
                {
                    if (BloomBuffUI.Transparency > 0.5f)
                    {
                        BloomBuffUI.Transparency -= 0.05f;
                    }
                    if (BloomBuffUI.Transparency < 0.5f)
                    {
                        BloomBuffUI.Transparency += 0.05f;
                    }
                }
            }

            HandleBloomBuffDuration();
            GivePlayerBloomBonus();

            //automatically remove all dragonfruit stacks if the player doesnt have that buff active
            if (!Dragonfruit)
            {
                DragonfruitStacks = 0;
            }
        }
    }
}