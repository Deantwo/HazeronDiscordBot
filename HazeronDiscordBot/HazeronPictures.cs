using System;
using System.Collections.Generic;
using System.Text;

namespace HazeronDiscordBot
{
    public static class HazeronPictures
    {
        public const string SohLogo = @"https://www.hazeron.com/images/SoHLogo.png";

        public const string Limbo = @"https://www.hazeron.com/wiki/images/a/ab/Limbo.gif";

        public static string GetGalaxyPicture(Galaxy galaxy)
        {
            switch (galaxy)
            {
                case Galaxy.AndromedaRising:
                    return @"https://www.hazeron.com/wiki/images/thumb/a/ab/Galaxy_AndromedaRising.png/200px-Galaxy_AndromedaRising.png";
                case Galaxy.BlackHole:
                    return @"https://www.hazeron.com/wiki/images/thumb/9/9e/Galaxy_BlackHole.png/200px-Galaxy_BlackHole.png";
                    case Galaxy.Core:
                    return @"https://www.hazeron.com/wiki/images/thumb/e/ea/Galaxy_Core.png/200px-Galaxy_Core.png";
                case Galaxy.CrownOfOthon:
                    return @"https://www.hazeron.com/wiki/images/thumb/e/e9/Galaxy_CrownOfOthon.png/200px-Galaxy_CrownOfOthon.png";
                case Galaxy.DyrathonsRetreat:
                    return @"https://www.hazeron.com/wiki/images/thumb/8/8f/Galaxy_DyrathonsRetreat.png/200px-Galaxy_DyrathonsRetreat.png";
                case Galaxy.EdgeOfTheRift:
                    return @"https://www.hazeron.com/wiki/images/thumb/5/5a/Galaxy_EdgeOfTheRift.png/200px-Galaxy_EdgeOfTheRift.png";
                case Galaxy.FallasEmbrace:
                    return @"https://www.hazeron.com/wiki/images/thumb/b/b6/Galaxy_FallasEmbrace.png/200px-Galaxy_FallasEmbrace.png";
                case Galaxy.FallenLegionsOfMuturon:
                    return @"https://www.hazeron.com/wiki/images/thumb/3/3d/Galaxy_FallenLegionsOfMuturon.png/200px-Galaxy_FallenLegionsOfMuturon.png";
                case Galaxy.HeartOfVictorus:
                    return @"https://www.hazeron.com/wiki/images/thumb/6/6a/Galaxy_HeartOfVictorus.png/200px-Galaxy_HeartOfVictorus.png";
                case Galaxy.HouseZanathar:
                    return @"https://www.hazeron.com/wiki/images/thumb/6/60/Galaxy_HouseZanathar.png/200px-Galaxy_HouseZanathar.png";
                case Galaxy.IndigoSea:
                    return @"https://www.hazeron.com/wiki/images/thumb/c/c7/Galaxy_IndigoSea.png/200px-Galaxy_IndigoSea.png";
                case Galaxy.InkarBorderRegion:
                    return @"https://www.hazeron.com/wiki/images/thumb/d/de/Galaxy_InkarBorderRegion.png/200px-Galaxy_InkarBorderRegion.png";
                case Galaxy.MilkyWay:
                    return @"https://www.hazeron.com/wiki/images/thumb/d/dc/Galaxy_MilkyWay.png/200px-Galaxy_MilkyWay.png";
                case Galaxy.MuturonEncounter:
                    return @"https://www.hazeron.com/wiki/images/thumb/7/7e/Galaxy_MuturonEncounter.png/200px-Galaxy_MuturonEncounter.png";
                case Galaxy.RansuulsFlamingSword:
                    return @"https://www.hazeron.com/wiki/images/thumb/a/a4/Galaxy_RansuulsFlamingSword.png/200px-Galaxy_RansuulsFlamingSword.png";
                case Galaxy.SevenTen:
                    return @"https://www.hazeron.com/wiki/images/thumb/4/4a/Galaxy_SevenTen.png/200px-Galaxy_SevenTen.png";
                case Galaxy.ShoresOfHazeron:
                    return @"https://www.hazeron.com/wiki/images/thumb/9/97/Galaxy_ShoresOfHazeron.png/200px-Galaxy_ShoresOfHazeron.png";
                case Galaxy.ThustrasEye:
                    return @"https://www.hazeron.com/wiki/images/thumb/c/c7/Galaxy_ThustrasEye.png/200px-Galaxy_ThustrasEye.png";
                case Galaxy.VeilOfTargoss:
                    return @"https://www.hazeron.com/wiki/images/thumb/3/39/Galaxy_VeilOfTargoss.png/200px-Galaxy_VeilOfTargoss.png";
                case Galaxy.VreenoxEclipse:
                    return @"https://www.hazeron.com/wiki/images/thumb/3/32/Galaxy_VreenoxEclipse.png/200px-Galaxy_VreenoxEclipse.png";
                case Galaxy.VulcansForge:
                    return @"https://www.hazeron.com/wiki/images/thumb/f/f6/Galaxy_VulcansForge.png/200px-Galaxy_VulcansForge.png";
                default:
                    return null;
            }
        }
    }
}
