using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainHelper
{
    public static List<Prototype> RemoveLocalMinima(Tile tile, List<Prototype> prototypes)
    {
        List<Prototype> result = new List<Prototype>();

        if (!tile.PointsUp)
        {
            foreach(Prototype p in prototypes)
            {
                if (p.meshName.Length == 0) // Internal or External
                {
                    result.Add(p);
                    continue;
                }

                string cliffStr = p.meshName.Split('-')[1]; // Ex: FFF-CCE-R
                if (tile.PointsUp)
                {
                    if (tile.a > 0 && cliffStr[(-p.rotation + 3) % 3] != 'E')
                        continue;
                    if (tile.b > 0 && cliffStr[(2 - p.rotation + 3) % 3] != 'E')
                        continue;
                    if (tile.c > 0 && cliffStr[(1 - p.rotation + 3) % 3] != 'E')
                        continue;
                }
                else
                {
                    if (tile.a < 0 && cliffStr[(-p.rotation + 3) % 3] != 'E')
                        continue;
                    if (tile.b < 0 && cliffStr[(2 - p.rotation + 3) % 3] != 'E')
                        continue;
                    if (tile.c < 0 && cliffStr[(1 - p.rotation + 3) % 3] != 'E')
                        continue;
                }

                result.Add(p);

            }
        }

        return result.Count == 0 ? prototypes : result;
    }
}
