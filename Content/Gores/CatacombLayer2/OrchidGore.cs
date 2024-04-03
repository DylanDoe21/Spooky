using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Gores.CatacombLayer2
{
	public class OrchidPinkBigGore1 : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			ChildSafety.SafeGore[gore.type] = true;
			gore.velocity = new Vector2(Main.rand.NextFloat() - 0.5f, Main.rand.NextFloat() * MathHelper.TwoPi);
			UpdateType = 910;
		}
	}
	public class OrchidPinkBigGore2 : OrchidPinkBigGore1
	{
	}
	public class OrchidPinkBigGore3 : OrchidPinkBigGore1
	{
	}
	public class OrchidPinkBigGore4 : OrchidPinkBigGore1
	{
	}
	public class OrchidPinkBigGore5 : OrchidPinkBigGore1
	{
	}


	public class OrchidPinkSmallGore1 : OrchidPinkBigGore1
	{
	}
	public class OrchidPinkSmallGore2 : OrchidPinkBigGore1
	{
	}
	public class OrchidPinkSmallGore3 : OrchidPinkBigGore1
	{
	}
	public class OrchidPinkSmallGore4 : OrchidPinkBigGore1
	{
	}
	public class OrchidPinkSmallGore5 : OrchidPinkBigGore1
	{
	}

	public class OrchidPurpleBigGore1 : OrchidPinkBigGore1
	{
	}
	public class OrchidPurpleBigGore2 : OrchidPinkBigGore1
	{
	}
	public class OrchidPurpleBigGore3 : OrchidPinkBigGore1
	{
	}
	public class OrchidPurpleBigGore4 : OrchidPinkBigGore1
	{
	}
	public class OrchidPurpleBigGore5 : OrchidPinkBigGore1
	{
	}

	public class OrchidPurpleSmallGore1 : OrchidPinkBigGore1
	{
	}
	public class OrchidPurpleSmallGore2 : OrchidPinkBigGore1
	{
	}
	public class OrchidPurpleSmallGore3 : OrchidPinkBigGore1
	{
	}
	public class OrchidPurpleSmallGore4 : OrchidPinkBigGore1
	{
	}
	public class OrchidPurpleSmallGore5 : OrchidPinkBigGore1
	{
	}
}