using System.Threading.Tasks;
using UnityEngine;

public class EntitySpawner: MonoBehaviour
{
    protected string _location = null;
    protected GameObject entity = null;

    public bool IsEntitySpawned { get => entity != null; }

    public EntitySpawner(string objectLocation)
    {
        _location = objectLocation;
    }

    async public virtual Task<GameObject> Spawn()
    {
        GameObject[] operation = await InstantiateAsync<GameObject>(Resources.Load<GameObject>(_location), transform);
        return operation[0];
    }

    //async public Task<string> Spawn(int i)
    //{
    //    await Task.Delay(3000);
    //    return $"Entity {i} spawned!";
    //}
}