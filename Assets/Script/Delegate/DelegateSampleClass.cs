using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateSampleClass : MonoBehaviour
{
	#region Delegate
	//Defination : Delegate is the type that can store the function inside of the instance

	//<Field Desc> Declaration of the delegation with "delegate" keyword
	public delegate void TestDelegate(); //delegate class that return void and take no parameter.

	public delegate bool BoolTestDelegate(int i); //Delegate class that return bool and take one int parameter.

    //<Field Desc> Create instanc of delegate class
    private TestDelegate testDelegateInstance;
	private BoolTestDelegate boolTestDelegateInstance;
	#endregion

	#region Action
	//Action is the bulid in C# for Delegate, but it only return null.
	private Action testAction; //action delegate class that return void and take no parameter (DEFAULT).
	private Action<int, float> testActionWithParam; //Action delegate calss that return void and take int and float as param (Generic Override).

	#endregion

	#region Func
	//Action is the bulid in C# for Delegate, able return and accept param
	private Func<bool> testFuncReturnBool; //A Func delegate that return bool and take no param;
	private Func<int,float ,bool> testFuncReturnBoolWith2Param; // A Func delegate that returb bool and take 2 param

	#endregion


	private void Start()
	{
		#region Assign Single
		//Assign single Function into the instance
		testDelegateInstance = TestDelegateSampleFunction;

		//Trigger the instance inside of the instance.
		testDelegateInstance();

		//Assign new Function into the instance, existing will be remove
		testDelegateInstance = TestDelegateSecondSampleFunction;

		//Trigger the instance inside of the instance.
		testDelegateInstance();
		testDelegateInstance = null;
		#endregion

		#region Assign Multiple
		testDelegateInstance += TestDelegateSampleFunction;
		testDelegateInstance += TestDelegateSecondSampleFunction;

		testDelegateInstance();

		//remove function from instance
		testDelegateInstance -= TestDelegateSecondSampleFunction;
		testDelegateInstance();

		testDelegateInstance = null;
		#endregion

		#region Assign Anoymous Method
		// -- Anoymous & Lamda expression method are nt suitable for the case that nned subscribe and unsubscribe due to inproper function reference

		testDelegateInstance = delegate () { Debug.Log("Anoymous Method"); };
		testDelegateInstance();

		//Use Lamda Expression

		testDelegateInstance = () => { Debug.Log("Lamda Expression"); };
		testDelegateInstance();

		boolTestDelegateInstance = (int num) => { return num < 5; };
		boolTestDelegateInstance(7);
		#endregion

		#region Assign Action
		testAction = () => { Debug.Log("Empty Action"); };
		testAction();

		testActionWithParam = (int i, float f) => { Debug.Log("Action with param " + i + " and " + f); };
		testActionWithParam(9, 1.1f);
		#endregion

		#region Assign Func
		testFuncReturnBool = () => { return false; };
		Debug.Log(testFuncReturnBool.ToString() + " returns " + testFuncReturnBool());

		testFuncReturnBoolWith2Param = (int i, float j) => { return i > j; };
		Debug.Log(testFuncReturnBoolWith2Param.ToString() + " returns " + testFuncReturnBoolWith2Param(1, 1.2f));
		#endregion



	}

	//<Field Desc> Create a function that match the "TestDelegate" Class
	private void TestDelegateSampleFunction()
	{
		Debug.Log("Test Delegate Sample");
	}

	private void TestDelegateSecondSampleFunction()
	{
		Debug.Log("Test Delegate Second Sample");
	}

	private bool BoolTestDelegateSampleFunction(int aaaa)
	{
		return aaaa != 0;
	}
}
