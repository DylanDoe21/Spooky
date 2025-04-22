using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.Fishing
{
    public class DumboOctopoid : ModItem 
    {
        public override void SetStaticDefaults() 
        {
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults() 
        {
            Item.CloneDefaults(ItemID.Bass);
        }
    }
}