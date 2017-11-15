using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyBase 
{

	void Init (BitmapCollision _bmpCol, PlayerController _player, EnemySpawner _spawn,BitmapCollision.LayerMask layerNum);
	void Die();
}
