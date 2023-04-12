using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientGatligator : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 30;
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;
            Item.width = 72;
            Item.height = 32;
            Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 38);
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
        }
    }
}