using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientClingerStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.mana = 20;
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;
            Item.width = 52;
            Item.height = 52;
            Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 0;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 45);
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
        }
    }
}