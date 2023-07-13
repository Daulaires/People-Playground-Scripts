using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mod
{
    public class Mod
    {
        public static void Main()
        {
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Knife"), // Assuming "Knife" is the name of the knife spawnable
                    NameOverride = "Sharp Knife",
                    DescriptionOverride = "A sharp knife that slices with any force.",
                    CategoryOverride = ModAPI.FindCategory("Weapons"),
                    // use default icon
                    ThumbnailOverride = ModAPI.LoadSprite("knife.png"), // Assuming "knife.png" is the name of the knife sprite
                    AfterSpawn = (Instance) =>
                    {
                        Instance.AddComponent<DamageOnCollision>();
                    }
                }
            );
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Spear"), // Assuming "Spear" is the name of the spear spawnable
                    NameOverride = "Sharp Spear",
                    DescriptionOverride = "A sharp spear that slices with any force.",
                    CategoryOverride = ModAPI.FindCategory("Weapons"),
                    // use default icon
                    ThumbnailOverride = ModAPI.LoadSprite("spear.png"), // Assuming "spear.png" is the name of the spear sprite
                    AfterSpawn = (Instance) =>
                    {
                        Instance.AddComponent<DamageOnCollision>();
                        ModAPI.CreateParticleEffect("Blood", Instance.transform.position);
                    }
                }
            );

            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Light Machine Gun"), // Assuming "Light Machine Gun" is the name of the Light Machine Gun spawnable
                    NameOverride = "Modified Machine Gun",
                    DescriptionOverride = "A machine gun that shoots FMJ.",
                    CategoryOverride = ModAPI.FindCategory("FireArms"),
                    AfterSpawn = (Instance) =>
                    {
                        // getting the Component
                        var gun = Instance.GetComponent<FirearmBehaviour>();


                        //Cartridge
                        Cartridge customCartridge = ModAPI.FindCartridge("5.56x45mm");
                        customCartridge.name = "5.56x45mm FMJ";
                        customCartridge.Damage = 10f;
                        customCartridge.StartSpeed = 300f;
                        customCartridge.PenetrationRandomAngleMultiplier = 0.1f;
                        customCartridge.Recoil = 2f;
                        customCartridge.ImpactForce = 0.00f;

                        // set the cartridge
                        gun.Cartridge = customCartridge;

                        // when the gun is fired
                        // make sure that there is a slider component on the gun
                        gun.gameObject.AddComponent<UseEventTrigger>().Action = () =>
                            {
                                // add the particle effect
                                ModAPI.CreateParticleEffect("MuzzleFlash", Instance.transform.position);
                            };
                        // don't change the original item
                        gun.gameObject.AddComponent<DamageOnCollision>();
                    }
                }
            );
        }
    }

    public class DamageOnCollision : MonoBehaviour
    {
        public void OnCollisionEnter2D(Collision2D collision)
        {
            Damage(collision);
        }
        // print the amount of damage dealt
        private void Damage(Collision2D collision)
        {
            // Adjust the impact force threshold as per your needs
            float ImpactForceThreshold = 0.5f;

            if (collision.relativeVelocity.magnitude > ImpactForceThreshold)
            {
                // Deal damage based on the relative velocity magnitude
                collision.gameObject.SendMessage("DamageType", "Stab", SendMessageOptions.DontRequireReceiver);
                ModAPI.CreateParticleEffect("Blood", collision.contacts[0].point);
            }
            // if the collision is the head, make it less likely to kill
            if (collision.gameObject.name == "Head")
            { 
                collision.gameObject.SendMessage("DamageType", "Stab", SendMessageOptions.DontRequireReceiver);
                ModAPI.CreateParticleEffect("Blood", collision.contacts[0].point);
            }
        }
    }
}
