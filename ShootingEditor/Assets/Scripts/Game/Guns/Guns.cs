using UnityEngine;
namespace Game
{
    abstract class Component
    {
        public abstract void Operation(Mover mover);
    }
    class Guns : Component
    {
        public override void Operation(Mover mover)
        {
        }
    }
    abstract class GunDecorator : Component
    {
        protected Component component;
        public void SetComponent(Component component)
        {
            this.component = component;
        }

        public override void Operation(Mover mover)
        {
            if (component != null)
            {
                component.Operation(mover);
            }
        }
    }
    class NormalGun : GunDecorator
    {
        float angle;
        Vector2 posOffset; // [-0.025, 0.025]

        public NormalGun()
        {
            _shotCounter = 0;
            posOffset = Vector2.zero;
        }
        public void Init(float angle, Vector2 posOffset)
        {
            this.angle = angle;
            this.posOffset = posOffset;
        }

        const int _shotInterval = 6;
        int _shotCounter = 0;
        public override void Operation(Mover mover)
        {
            base.Operation(mover);
            if (_shotCounter == 0)
            {
                CreateShot(mover);
            }
            _shotCounter = (_shotCounter + 1) % _shotInterval;
        }

        private void CreateShot(Mover mover)
        {
            TeamShot shot = GameSystem._Instance.CreateShot<TeamShot>();
            float x = mover._X + posOffset.x;
            float y = mover._Y + posOffset.y;
            float velocity = 0.04f;
            shot.Init(PlayName.obj_shot, x, y, angle, velocity);
        }
    }
}