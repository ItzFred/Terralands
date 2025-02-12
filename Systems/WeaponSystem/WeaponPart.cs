using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Terralands.Systems.WeaponSystem
{
    public abstract class WeaponPart : ModType
    {
        public WeaponPart()
        {

        }

        public WeaponPart(int type)
        {
            WeaponPart part = WeaponPartMadeID.IDs[type];
            partType = part.partType;
            connectionPoints = part.connectionPoints;
        }

        public virtual int Type { get; set; }
        /// <summary>
        /// The type of part - specifies which type of weapon it is for and where it attaches. Use WeaponPartID for this.
        /// </summary>
        public virtual byte partType {get; set;}

        public virtual string Texture => (base.GetType().Namespace + "." + this.Name).Replace('.', '/');

        /// <summary>
        /// The material map for this texture. Use only one of three colors for it -> (255,0,0) (0,255,0) (0,0,255). 
        /// Use the same texture name with the suffix _Material
        /// </summary>
        public virtual string MaterialTexture => (base.GetType().Namespace + "." + this.Name).Replace('.', '/') + "_Material";

        /// <summary>
        /// Add parts that connect to this part and the position on the texture they connect to.
        /// </summary>
        public virtual Dictionary<byte, Vector2> connectionPoints { get; set; }

        public bool drawn = false;
        public Vector2 drawnOffset = Vector2.Zero;

        public override void Load()
        {
            WeaponPartMadeID.IDs = WeaponPartMadeID.IDs.Append(this).ToArray();
        }
        protected override void Register()
        {
            ModTypeLookup<WeaponPart>.Register(this);
        }
    }

    public static class WeaponPartMadeID
    {
        public static WeaponPart[] IDs = new WeaponPart[] { };

        public static int GetType(WeaponPart part)
        {
            for (int i = 0; i < IDs.Length; i++)
            {
                if (part.Name == IDs[i].Name) return i;
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
