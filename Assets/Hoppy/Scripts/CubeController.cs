using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeController : MonoBehaviour {

	#region 变量定义

	// 第一个板子
	public GameObject cube;
	// 场景中同时出现的板子数量
	public int numberOfInstantiatedCubes;

	// 保存板子的数组
	private GameObject[] instantiatedCubes;
	// 保存板子的队列
	private Queue<GameObject> queueOfCubes = new Queue<GameObject>();

	// 板子上的钻石prefab
	public GameObject gems;
	// 保存钻石的队列
	private Queue<GameObject> queueOfGems = new Queue<GameObject>();

	// 板子的颜色
	public Color currentColor;
	// 保存板子的颜色数组
	public Color[] colorsOfTheCube;

	// 板子在水平方向的移动速度
	public float horizontalSpeed;
	// 新生成的板子降落的速度
	public float slidingSpeed;

	// 边缘
	public float margin;
	// 位置坐标中x方向的值数组
	public int[] xPositions;
	// 板子在z方向的间隔
	public int lengthOfTheCubes;
	// 第一块板子在z方向的值
	float actualZPosition = 0.0f;
	// 颜色列表
	private List<Color> colorsList;
	// 用于改变板子颜色的计数器 
	private int counterForSpawnCubes = 1;

	// 随即一个值用来决定该板子是否在x方向移动
	private int randomForMovingTheCubeInXaxis;

	// player引用
	public GameObject player;
    // Game Controller和player之间的位移
    private float offset;

	#endregion



	#region Unity回调

	void Start ()
	{
        //Game Controller和player在z轴方向的位移
        offset = transform.position.z - player.transform.position.z;
		// 初始化颜色列表
		colorsList = new List<Color>(colorsOfTheCube.Length);

		// 创建板子数组并指定第一个板子     
		instantiatedCubes = new GameObject[numberOfInstantiatedCubes+1];
		instantiatedCubes[0] = cube;

		// 用颜色数组里的值填充颜色列表 
		FillColors();
		// 初始化和创建第一个板子
		StartCoroutine(OnStart());

		// 初始化和创建钻石
		for (int i= 0;i <= 9; i++)
		{
			GameObject instantiatedGems = Instantiate(gems) as GameObject;
			instantiatedGems.SetActive(false);
			instantiatedGems.name = "Gem";
			queueOfGems.Enqueue(instantiatedGems);
		}                
	}

	void LateUpdate()
	{
        // 使Game Controller和player在z方向保持一定距离
        if (player != null)
			transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z + offset);

	}

	#endregion



	#region 开始时把板子放在指定位置

	IEnumerator OnStart()
	{
		for (int i = 0; i < numberOfInstantiatedCubes; i++)
		{
			// 从确定的x位置中选择一个
			int randomSelectionForXPosition = UnityEngine.Random.Range(0, xPositions.Length);
			int currentXPosition = xPositions[randomSelectionForXPosition];

			// 从选择的位置随机偏移一点
			float actualXPosition = currentXPosition + UnityEngine.Random.Range(-margin, margin);

			// 初始化板子的y
			float yPosition = (((i + 1f) * numberOfInstantiatedCubes) / numberOfInstantiatedCubes);

			// 根据前面的计算得到板子的位置
			Vector3 place = new Vector3
				(actualXPosition,
					yPosition,
					actualZPosition + lengthOfTheCubes
				);

			// 创建一个板子并放到计算好的位置
			GameObject instantiatedCube = Instantiate(cube, place, Quaternion.identity) as GameObject;

			// 设置一下板子的名字没鸟用 
			instantiatedCube.name = "Cube";
			// 设置板子的颜色为默认颜色
			instantiatedCube.GetComponentInChildren<Renderer>().material.color = currentColor;

			// 访问数组中的下一个元素
			instantiatedCubes[i + 1] = instantiatedCube;
			// 计算一下板子的z坐标值
			actualZPosition = actualZPosition + lengthOfTheCubes;
		}

		yield return new WaitForSecondsRealtime(0.1f);

		// 把板子从上落下来
		for (int i = 1; i <= numberOfInstantiatedCubes; i++)
		{
			StartCoroutine(slidingDownTheCubes(instantiatedCubes[i], null));

		}
	}

	#endregion



	#region 生成板子和钻石，让板子降落到水平位置

	public void spawnCubes ()
	{
		// 改变板子的颜色计数
		counterForSpawnCubes++;

		// 检查当前生成的板子颜色是否会变
		if (counterForSpawnCubes == 20)
		{
			changeColor();
			counterForSpawnCubes = 0;
		}

		// 用于检查钻石是否实例化的标志
		bool gemInstantiated = false;


		// 调整新创建的板子的位置
		actualZPosition = actualZPosition + lengthOfTheCubes; 
		int randomSelectionForXPosition = UnityEngine.Random.Range(0, xPositions.Length);
		int currentXPosition = xPositions[randomSelectionForXPosition];
		float actualXPosition = currentXPosition + UnityEngine.Random.Range(-margin, margin);
		Vector3 place = new Vector3
			(actualXPosition,
				5.0f,
				actualZPosition
			);

		// 出队列一个板子，设置颜色摆放好位置
		GameObject instantiatedCube = queueOfCubes.Dequeue();
		instantiatedCube.transform.position = place;
		instantiatedCube.GetComponent<Renderer>().material.color = currentColor;
		instantiatedCube.SetActive(true);

		// 检查是否应该生成钻石
		int randomNumberToSpawnGems = UnityEngine.Random.Range(0,11);
		if (randomNumberToSpawnGems >= 10)
		{
			gemInstantiated = true;
			// 出队列一个钻石并放在板子上
			GameObject gem = queueOfGems.Dequeue();
			gem.transform.position = new Vector3(place.x,(place.y + 0.6f),place.z);
			gem.SetActive(true);
			// 用协程把板子和钻石降落下来
			StartCoroutine(slidingDownTheCubes(instantiatedCube, gem));
		}

		if (!gemInstantiated) 
			// 用协程把板子降落下来
			StartCoroutine(slidingDownTheCubes(instantiatedCube,null));
	}

	IEnumerator slidingDownTheCubes(GameObject instantiatedCube, GameObject gem)
	{
		// 一个根据板子降落的高度而变化的相对速度
		float relativeSpeed;

		// 板子和钻石会降落下来
		if (gem != null)
		{
			while (instantiatedCube.transform.position.y >= 0.1f)
			{
				// 根据板子降落的高度改变速度
				if (instantiatedCube.transform.position.y <= 0.5f)
					relativeSpeed = 0.5f;
				else relativeSpeed = instantiatedCube.transform.position.y;

				instantiatedCube.transform.Translate(Vector3.down * 2 * relativeSpeed * slidingSpeed * Time.deltaTime);
				gem.transform.Translate(Vector3.down * relativeSpeed * 2 * slidingSpeed * Time.deltaTime);
				yield return null;
			}

			//最后精确调整一下板子的位置
			instantiatedCube.transform.position = new Vector3(instantiatedCube.transform.position.x,
				0,
				instantiatedCube.transform.position.z
			);
			gem.transform.position = new Vector3(instantiatedCube.transform.position.x,
				0.6f,
				instantiatedCube.transform.position.z
			);

			// 板子有20%的几率左右移动
			randomForMovingTheCubeInXaxis = UnityEngine.Random.Range(0, 9);
			if (randomForMovingTheCubeInXaxis >= 8)
				StartCoroutine(moveCube(instantiatedCube, gem, instantiatedCube.transform.position.x, instantiatedCube.transform.position.z));
			else
			{
				float z = instantiatedCube.transform.position.z;
				while (z == instantiatedCube.transform.position.z) yield return null;
				gem.transform.position = new Vector3(0, 0, 0);
				gem.SetActive(false);
				queueOfGems.Enqueue(gem);
				yield break;
			}
		}
		// 只降落板子
		else
		{
			while (instantiatedCube.transform.position.y >= 0.1f)
			{
				if (instantiatedCube.transform.position.y <= 0.5f)
					relativeSpeed = 0.5f;
				else relativeSpeed = instantiatedCube.transform.position.y;

				instantiatedCube.transform.Translate(Vector3.down * 2 * relativeSpeed * slidingSpeed * Time.deltaTime);
				yield return null;
			}

			instantiatedCube.transform.position = new Vector3(instantiatedCube.transform.position.x,
				0,
				instantiatedCube.transform.position.z
			);

			randomForMovingTheCubeInXaxis = UnityEngine.Random.Range(0, 9);
			if (randomForMovingTheCubeInXaxis >= 8)
				StartCoroutine(moveCube(instantiatedCube, null, instantiatedCube.transform.position.x, instantiatedCube.transform.position.z));
		}
	}

	// 水平移动板子和钻石
	IEnumerator moveCube(GameObject cubeWillMove, GameObject gemWillMove, float displacementInXAxis, float displacementInZAxis)
	{
		float z = cubeWillMove.transform.position.z;
		if (gemWillMove != null)                                                                                                                   
		{
            //检查板子是否已经回收
			while (z == cubeWillMove.transform.position.z)                                                                                          
			{
                //向右移动板子
				while ((cubeWillMove.transform.position.x <= displacementInXAxis + margin) && (z == cubeWillMove.transform.position.z))             
				{
					cubeWillMove.GetComponent<Transform>().Translate(Vector3.right * Time.deltaTime * horizontalSpeed);
					gemWillMove.transform.position = new Vector3(cubeWillMove.transform.position.x, 0.6f, gemWillMove.transform.position.z);
					yield return null;
				}

                //向左移动板子
				while ((cubeWillMove.transform.position.x > displacementInXAxis - margin) && (z == cubeWillMove.transform.position.z))              
				{
					cubeWillMove.GetComponent<Transform>().Translate(Vector3.left * Time.deltaTime * horizontalSpeed);
					gemWillMove.transform.position = new Vector3(cubeWillMove.transform.position.x, 0.6f, gemWillMove.transform.position.z);
					yield return null;
				}
			}

			// 板子回收后钻石也不在依附
			gemWillMove.transform.position = new Vector3(0, 0, 0);
			gemWillMove.SetActive(false);
			queueOfGems.Enqueue(gemWillMove);                                                                                                           
			yield break;
		}
		else                                                                                                                                            
		{
			while (z == cubeWillMove.transform.position.z)
			{
				while ((cubeWillMove.transform.position.x <= displacementInXAxis + margin) && (z == cubeWillMove.transform.position.z))                 
				{
					cubeWillMove.GetComponent<Transform>().Translate(Vector3.right * Time.deltaTime * horizontalSpeed);
					yield return null;
				}
				while ((cubeWillMove.transform.position.x > displacementInXAxis - margin) && (z == cubeWillMove.transform.position.z))                  
				{
					cubeWillMove.GetComponent<Transform>().Translate(Vector3.left * Time.deltaTime * horizontalSpeed);
					yield return null;
				}
			}
			yield break;
		}

	}

	#endregion



	#region 颜色相关的方法

	void changeColor()
	{
		Color randomColor;
		// 从颜色列表里移除当前颜色然后在剩下的颜色里选取一个随机颜色
		colorsList.Remove(currentColor);
		// 如果颜色列表为空重新填充颜色
		if (colorsList.Count <= 0) FillColors();
		// 如果新随机的颜色和当前颜色相同重新随机
		do
		{
			int selectRandomColor = UnityEngine.Random.Range(0, colorsList.Count);
			randomColor = colorsList[selectRandomColor];
		} while (randomColor == currentColor);
		currentColor = randomColor; 

		// 改变板子的颜色
		foreach(GameObject cube in instantiatedCubes)
		{
			cube.GetComponent<Renderer>().material.color = currentColor;
		}
	}

	// 把颜色数组里的颜色填充到颜色列表里
	void FillColors()
	{
		if (colorsList.Count > 0)
			colorsList.Clear();
		for (int i = 0; i <= 2; i++)
		{
			colorsList.Add(colorsOfTheCube[i]);
		}
	}

	#endregion



	#region 用来回收板子的碰撞检测

	void OnTriggerEnter(Collider other)
	{
        // 检查Game Controller是否碰到了板子
        if (other.tag == "cube")
		{
			other.gameObject.transform.position = Vector3.zero;
			queueOfCubes.Enqueue(other.gameObject);
			other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
			other.gameObject.SetActive(false);
			spawnCubes();
		}
	}

	#endregion
}