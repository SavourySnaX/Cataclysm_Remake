using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public interface ITriggerBase
{
	void Init(BitmapCollision bmpCol);
	void Trigger();

	void SetupBase(Vector3 wPos);
	void SetupTrigger (Vector3 wPos);

}
