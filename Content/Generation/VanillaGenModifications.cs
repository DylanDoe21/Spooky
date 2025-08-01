using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using ReLogic.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Spooky.Content.Generation
{
	public class VanillaGenModifications : ModSystem
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			//re-locate the jungle temple so it doesnt get generated over by the catacombs
			int JungleTempleIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Temple"));
			tasks[JungleTempleIndex] = new PassLegacy("Jungle Temple", (progress, config) =>
			{
				int MinimumY = (int)Main.rockLayer;
				int MaximumY = Main.maxTilesY - 500;
				if (MinimumY > MaximumY - 1)
				{
					MinimumY = MaximumY - 1;
				}
				int newTempleY = WorldGen.genRand.Next(MinimumY, MaximumY);

				//middle of the where the cemetery/catacombs is
				int XStart = Catacombs.PositionX - (Cemetery.BiomeWidth / 2);
				int XMiddle = Catacombs.PositionX;

				//attempt to find a valid position for the jungle temple to place in, just in case it generates far away from the jungle
				bool foundValidPosition = false;
				int attempts = 0;

				//keep moving towards the center of the world until a valid position in the jungle is found
				while (!foundValidPosition && attempts++ < 100000)
				{
					while (Catacombs.NoJungleNearby(XMiddle, newTempleY))
					{
						XMiddle += (XMiddle > (Main.maxTilesX / 2) ? -100 : 100);
					}
					if (!Catacombs.NoJungleNearby(XMiddle, newTempleY))
					{
						foundValidPosition = true;
					}
				}

				//define the x-position and then place the temple after finding a valid position
				int newTempleX = XMiddle < (Main.maxTilesX / 2) ? XMiddle + 450 : XMiddle - 450;

				WorldGen.makeTemple(newTempleX, newTempleY);
			});

			//re-locate the shimmer to be closer to the edge of the world so it also never gets generated over by the catacombs or rotten depths when necessary
			//copy-pasted and slightly modified shimmer generation code from terraria itself
			int shimmerIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shimmer"));
			tasks[shimmerIndex] = new PassLegacy("Shimmer", (progress, config) =>
			{
				int ShimmerYMin = (int)(Main.worldSurface + Main.rockLayer) / 2 + 100;
				int ShimmerYMax = (int)(((Main.maxTilesY - 250) * 2) + Main.rockLayer) / 3;

				bool RottenDepthsJungleSide = (GenVars.JungleX < (Main.maxTilesX / 2) && ZombieOcean.StartPositionX < (Main.maxTilesX / 2)) ||
				(GenVars.JungleX > (Main.maxTilesX / 2) && ZombieOcean.StartPositionX > (Main.maxTilesX / 2));

				//if the rotten depths is on the jungle side, lower the shimmer more
				if (RottenDepthsJungleSide)
				{
					ShimmerYMin = Main.maxTilesY < 1800 ? Main.maxTilesY - 250 : (Main.maxTilesY / 2) + 30;
				}

				if (ShimmerYMax > Main.maxTilesY - 200)
				{
					ShimmerYMax = Main.maxTilesY - 200;
				}
				if (ShimmerYMax <= ShimmerYMin)
				{
					ShimmerYMax = ShimmerYMin + 50;
				}

				int ShimmerX = GenVars.dungeonSide < 0 ? Main.maxTilesX - 100 : 100;
				int ShimmerY = WorldGen.genRand.Next(ShimmerYMin, ShimmerYMax);

				int ShimmerYAnniversaryMin = (int)Main.worldSurface + 150;
				int ShimmerYAnniversaryMax = (int)(Main.rockLayer + Main.worldSurface + 200) / 2;

				if (ShimmerYAnniversaryMax <= ShimmerYAnniversaryMin)
				{
					ShimmerYAnniversaryMax = ShimmerYAnniversaryMin + 50;
				}

				if (WorldGen.tenthAnniversaryWorldGen)
				{
					ShimmerY = WorldGen.genRand.Next(ShimmerYAnniversaryMin, ShimmerYAnniversaryMax);
				}

				while (!WorldGen.ShimmerMakeBiome(ShimmerX, ShimmerY))
				{
					//this changes the shimmer position to be closer to the edge of the world
					ShimmerX = (GenVars.dungeonSide < 0) ? (int)(Main.maxTilesX * 0.95f) : (int)(Main.maxTilesX * 0.05f);
					ShimmerY = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2 + 22, ShimmerYMax);
				}

				GenVars.shimmerPosition = new Vector2D(ShimmerX, ShimmerY);

				//add the shimmer as a protected structure so nothing attempts to generate over it
				int ProtectionRectSize = 200;
				GenVars.structures.AddProtectedStructure(new Rectangle(ShimmerX - ProtectionRectSize / 2, ShimmerY - ProtectionRectSize / 2, ProtectionRectSize, ProtectionRectSize));
			});
		}
	}
}