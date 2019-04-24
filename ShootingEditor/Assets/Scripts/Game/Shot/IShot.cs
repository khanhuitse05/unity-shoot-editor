using System.Collections;

namespace Game
{
    public interface IShot
    {
        IEnumerator Shot(Mover mover);
        string getDescription();
    }

    public class ShotFactory
    {
        public ShotFactory()
        {

        }

        public IShot CreateShot(ShotName name)
        {
            switch (name)
            {
                case ShotName.shotFormBorder:
                    return new Shot_FormBorder();
                case ShotName.shotFormCorner:
                    return new Shot_FormCorner();
                case ShotName.circle:
                    return new Shot_Circle();
                case ShotName.circles:
                    return new Shot_Circles();
                case ShotName.placedCircle:
                    return new Shot_PlacedCircle();
                case ShotName.randomCircles:
                    return new Shot_RandomCircles();
                case ShotName.randomAngleCircle:
                    return new Shot_RandomAngleCircle();
                case ShotName.nWay:
                    return new Shot_NWay();
                case ShotName.nWays:
                    return new Shot_NWays();
                case ShotName.spirals:
                    return new Shot_Spirals();
                case ShotName.biDirectionalSpirals:
                    return new Shot_BiDirSpiral();
                case ShotName.aimimg:
                    return new Shot_Aiming();
                case ShotName.aimingLines:
                    return new Shot_AimingLines();
                case ShotName.aimingNWayLines:
                    return new Shot_AimingNWayLine();
                case ShotName.aimingRollings:
                    return new Shot_AimingRollings();
                case ShotName.rollingNWay:
                    return new Shot_RollingNWay();
                case ShotName.randomSpread:
                    return new Shot_RandomSpread();
                default:
                    break;
            }
            return null;
        }
    }
    public enum ShotName
    {
        shotFormBorder = 0,
        shotFormCorner,
        circle,
        circles,
        placedCircle,
        randomCircles,
        randomAngleCircle,
        nWay,
        nWays,
        spirals,
        biDirectionalSpirals,
        aimimg,
        aimingLines,
        aimingNWayLines,
        aimingRollings,
        rollingNWay,
        randomSpread,
        //pattern,
    }
}