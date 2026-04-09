using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BusinessLogic.Awaiter
{
    public class AllSpawnersAwaiter
    {
        async public Task<List<GameObject>> AwaitFor(EntitySpawner[] spawners)
        {
            List<GameObject> result = new List<GameObject>();
            for (int i = 0; i < spawners.Length; i++)
            {
                result.Add(await spawners[i].Spawn());
            }
            return result;
        }
        //async public Task<List<string>> AwaitFor(EntitySpawner[] spawners)
        //{
        //    List<string> result = new List<string>();
        //    for (int i = 0; i < spawners.Length; i++)
        //    {
        //        result.Add(await spawners[i].Spawn(i));
        //    }

        //    return result;
        //}

    }
}
