using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientUmbrella : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 22;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.width = 60;
            Item.height = 72;
            Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 12);
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
        }
    }
}