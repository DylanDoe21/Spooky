using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Gores.Quest
{
	public class EyeWizardClothGore1 : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			ChildSafety.SafeGore[gore.type] = true;
			gore.velocity = new Vector2(Main.rand.NextFloat() - 0.5f, Main.rand.NextFloat() * MathHelper.TwoPi);
			UpdateType = 910;
		}
	}

	public class EyeWizardClothGore2 : EyeWizardClothGore1
	{
	}

	public class EyeWizardClothGore3 : EyeWizardClothGore1
	{
	}
	
	public class EyeWizardClothGore4 : EyeWizardClothGore1
	{
	}

	public class EyeWizardClothGore5 : EyeWizardClothGore1
	{
	}

	public class EyeWizardClothGore6 : EyeWizardClothGore1
	{
	}

	public class EyeWizardClothGore7 : EyeWizardClothGore1
	{
	}

	public class EyeWizardClothGore8 : EyeWizardClothGore1
	{
	}

	public class EyeWizardClothGore9 : EyeWizardClothGore1
	{
	}
}