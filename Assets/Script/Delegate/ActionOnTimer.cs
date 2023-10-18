using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ActionOnTimer : MonoBehaviour
{
	private Action timerCallback;
    private float timer;

	private void Update()
	{
		timer -= Time.deltaTime;
		if (IsTimerComplete() && timerCallback != null)
		{
			timerCallback();
			timerCallback = null;
		}
	}

	public void SetTimer(float timer, Action timerCallback)
	{
		this.timer = timer;
		this.timerCallback = timerCallback;
	}

	public bool IsTimerComplete()
	{
		return timer <= 0;
	}
}
