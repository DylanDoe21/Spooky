using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace Spooky.Content.Gores.Misc
{
	public class TarBombGore1 : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			ChildSafety.SafeGore[gore.type] = true;
			gore.timeLeft = 2;
		}
	}

	public class TarBombGore2 : TarBombGore1
	{
	}

	public class TarBombGore3 : TarBombGore1
	{
	}

	public class TarBombGore4 : TarBombGore1
	{
	}

	public class TarBombGore5 : TarBombGore1
	{
	}

	public class TarBombGore6 : TarBombGore1
	{
	}
}