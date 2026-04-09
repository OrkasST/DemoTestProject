using System.Threading.Tasks;
using UnityEngine;

public class EnemySpawner : EntitySpawner
{
    public EnemySpawner(string location) : base(location) { }

    async public override Task<GameObject> Spawn()
    {
        var Enemy = await base.Spawn();
        Enemy.transform.localPosition = Vector3.zero;

        var hpBar = Enemy.GetComponentInChildren<HPBar>();
        hpBar.Entity = Enemy;
        hpBar.Initialize();

        return Enemy;
    }
}
