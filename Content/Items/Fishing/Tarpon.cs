using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.Fishing
{
    public class Tarpon : ModItem 
    {
        public override void SetStaticDefaults() 
        {
            Item.ResearchUnlockCount = 2;
        }

        public override void SetDefaults() 
        {
            Item.CloneDefaults(ItemID.Batfish);
        }

        public override bool IsQuestFish() 
        {
            return true;
        }

        public override void AnglerQuestChat(ref string description, ref string catchLocation) 
        {
            description = Language.GetTextValue("Mods.Spooky.AnglerQuest.Tarpon.Description");
            catchLocation = Language.GetTextValue("Mods.Spooky.AnglerQuest.Tarpon.CatchLocation");
        }
    }
}