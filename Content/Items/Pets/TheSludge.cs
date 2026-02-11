using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.Pets
{
    public class TheSludge : ModItem 
    {
        public override void SetDefaults() 
        {
            Item.CloneDefaults(ItemID.Bass);
        }
    }
}