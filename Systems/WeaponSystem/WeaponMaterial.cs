using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Terralands.Systems.WeaponSystem
{
    public abstract class WeaponMaterial : ModType
    {
        public virtual int Type { get; set; }

        /// <summary>
        /// Set to true if you want the texture to tile with other parts horizontally (This will put the texture out of place and make things mixelly :( )
        /// </summary>
        public virtual bool TileHorizontal {  get; set; }
        public virtual string Texture => (base.GetType().Namespace + "." + this.Name).Replace('.', '/');

        public override void Load()
        {
            WeaponMaterialID.IDs = WeaponMaterialID.IDs.Append(this).ToArray();
        }
        protected override void Register()
        {
            ModTypeLookup<WeaponMaterial>.Register(this);
        }
    }

    public static class WeaponMaterialID
    {
        public static WeaponMaterial[] IDs = new WeaponMaterial[] { };

        public static int GetType(WeaponMaterial material)
        {
            for (int i = 0; i < IDs.Length; i++)
            {
                if (material.Name == IDs[i].Name) return i;
            }

            return 0;
        }

        public static int GetType(string name)
        {
            for (int i = 0; i < IDs.Length; i++)
            {
                if (name == IDs[i].Name) return i;
            }

            return 0;
        }
    }
}
