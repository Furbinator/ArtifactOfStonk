using System;
using BepInEx;
using RoR2;
using UnityEngine;
using R2API;
using R2API.Utils;

namespace Furbinator
{
	[BepInDependency("com.bepis.r2api")]
	[BepInPlugin("com.Furbinator.ArtifactOfStonk", "Artifact of Stonk", "1.0.0")]
	[R2APISubmoduleDependency(nameof(LoadoutAPI))]
	public class ArtifactOfStonk : BaseUnityPlugin
	{
		ArtifactDef Stonk = ScriptableObject.CreateInstance<ArtifactDef>();
		public void Awake()
		{
			Stonk.nameToken = "Artifact of Stonk";
			Stonk.descriptionToken = "Every hit gives you items, getting hurt takes them away";
			Stonk.smallIconDeselectedSprite = LoadoutAPI.CreateSkinIcon(Color.white, Color.white, Color.white, Color.white);
			Stonk.smallIconSelectedSprite = LoadoutAPI.CreateSkinIcon(Color.grey, Color.white, Color.white, Color.white);

			ArtifactCatalog.getAdditionalEntries += (list) =>
			{
				list.Add(Stonk);
			};

			On.RoR2.CharacterBody.OnTakeDamageServer += CharacterBody_OnTakeDamageServer;

		}

		private void CharacterBody_OnTakeDamageServer(On.RoR2.CharacterBody.orig_OnTakeDamageServer orig, CharacterBody self, DamageReport damageReport)
		{
			orig(self, damageReport);
			if (RunArtifactManager.instance.IsArtifactEnabled(Stonk.artifactIndex))
			{           // If an ALLY DEALS DAMAGE
				if (damageReport.attackerBody)
				{
					if (damageReport.attackerBody.teamComponent.teamIndex == TeamIndex.Player && damageReport.damageInfo.procCoefficient > UnityEngine.Random.value)
					{
						damageReport.attackerBody.inventory.GiveRandomItems(1);
					}
					else if (self.teamComponent.teamIndex == TeamIndex.Player)
					{
						int count = self.inventory.itemAcquisitionOrder.Count;
						for (int i = count; i > 0; i--)
						{
							if (UnityEngine.Random.value > 0.5f)
							{
								self.inventory.RemoveItem(self.inventory.itemAcquisitionOrder[i], self.inventory.GetItemCount(self.inventory.itemAcquisitionOrder[i]));
							}
						}
					}
				}
			}
		}
	}
}
