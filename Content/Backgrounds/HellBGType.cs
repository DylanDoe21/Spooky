using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Backgrounds
{
    public abstract class HellBGType : IAutoload
    {
        public int ID;
        public float Transparency;
        void IAutoload.Load()
        {
            if (Main.dedServ)
            {
                return;
            }
            HellBGManager.AddHellBG(this);
            OnLoad();
        }

        void IAutoload.Unload()
        {
            OnUnload();
        }

        public abstract bool IsActive();
        public abstract string TexturePath { get; }

        public virtual float TransitionSpeed => 0.02f;
        public virtual Color DrawColor => new Color(255, 255, 255, 255);

        protected virtual void OnLoad()
        {
        }

        protected virtual void OnUnload()
        {
        }

        public virtual bool PreDraw(Texture2D texture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, float Scale, int Layer)
        {
            return true;
        }

        public virtual void PostDraw(Texture2D texture, Vector2 drawPosition, Rectangle frame, Color drawColor, float Scale, int Layer)
        {
        }
    }
}