using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    #region 变量定义

    public Rigidbody playerRigidbody;

    private float speedFactor;

    public Slider speedFactorSlider;

    public float targetXSpeed;
	// 碰到钻石的特效Prefab
    public GameObject gemsExplosion;

    public GameObject[] ballsAry;

	// 球在x轴向的滑动速度
    private float slidingSpeed = 0f;

    private float minDis = 1f;

    // 球在x方向的位置限制
    private float xPosLimit = 5f;
	// 在z轴向的跳跃距离
	private float jumpDistance = 4f;
	// 在Y轴向的跳跃高度
	private float jumpHeight = 2.0f;
	// 最小重力对应最小速度
	private float minGravity = -35;
	// 最大重力对应最大速度
	private float maxGravity = -85;
	// 多少步之后重力会增加 
	private int gravityRate = 10;
	// 增加多少重力每速度步进
	private int gravityStep = 10;

	// 游戏开始状态标志
	private bool gameStarted = false;
	// 游戏结束状态标志
	private bool gameOver = false;

	// 是否启用键盘
	private bool useKeyboard = true;

	// 每步获得多少分
	private int score = -1;

	public GamePlayUIController uiController;

	#endregion

	#region Unity回调

	void Start ()
	{
		// 游戏开始后再启用重力
		Physics.gravity = new Vector3(0, 0, 0);

        speedFactor = PlayerPrefs.GetFloat("SpeedFactor", 0.35f);

        speedFactorSlider.value = speedFactor;

        speedFactorSlider.onValueChanged.AddListener(OnSpeedFactorChanged);

        initCurBall();
    }

	void Update ()
	{
        // 使用触摸输入键盘和控制滑动
        controlSliding();

        // 控制小球旋转
        controlBallRotation();

        // Check whether game is over or not.
        checkGameOver();
    }

    void controlSliding()
    {
        if (gameStarted && !gameOver)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    if (Input.GetTouch(0).deltaPosition.sqrMagnitude > minDis)
                    {
                        slidingSpeed = Input.GetTouch(0).deltaPosition.x;
                    }
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Stationary)
                {
                    slidingSpeed = 0f;
                }
            }
            else if (useKeyboard)
            {
                // 用键盘的左右方向键控制滑动
                float moveHorizontal = Input.GetAxis("Horizontal");

                slidingSpeed = moveHorizontal * 7;
            }

            targetXSpeed = Mathf.Lerp(playerRigidbody.velocity.x, slidingSpeed * speedFactor * 10f, Time.deltaTime);
            // 限制小球的位置在xPosLimit和-xPosLimit之间
            playerRigidbody.velocity = new Vector3(targetXSpeed, playerRigidbody.velocity.y, playerRigidbody.velocity.z);

            // 限制小球的位置在xPosLimit和-xPosLimit之间
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -xPosLimit, xPosLimit), transform.position.y, transform.position.z);
        }
    }

    #endregion

    #region 游戏状态控制

    public void startGame ()
	{
		if (!gameStarted)
		{
			// 用最小重力值启用重力
			Physics.gravity = new Vector3(0, minGravity, 0);

			uiController.onGameStarted ();
			gameStarted = true;
		}
	}

	void checkGameOver ()
	{
		if (transform.position.y < 0 && !gameOver)
		{
            // 游戏结束
            SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
            // 停止所有力
            playerRigidbody.velocity = Vector3.zero;

			// 增加重力
			Physics.gravity = new Vector3(0, maxGravity -50.0f, 0);


			// 更新UI显示和游戏状态
			uiController.onGameOver ();
			gameOver = true;


			// 删除小球
			Destroy (gameObject, 1f);
		}
	}

    #endregion

    #region 小球控制

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "cube" && gameStarted && !gameOver)
        {
            // 碰到板子
            SoundManager.Instance.PlaySound(SoundManager.Instance.jump);
            score++;                             
            uiController.score = score;          
            uiController.updateScoreUITexts();

            //根据新的得分重新计算重力
            controlGravity();        

            // 小球碰到板子时的默认y轴位置
            float defultYPos = GetComponent<Collider>().bounds.size.y / 2 + col.gameObject.GetComponent<Collider>().bounds.size.y / 2;

            // 修正因碰撞检测不及时造成的错误位置
            transform.position = new Vector3(transform.position.x, defultYPos, col.gameObject.transform.position.z);

            // 显示板子的阴影
            col.gameObject.transform.GetChild(0).gameObject.SetActive(true);


			// 取得当前的重力值
            float g = Physics.gravity.magnitude;

			// 计算跳到固定高度需要的时间
            float totalTime = Mathf.Sqrt(jumpHeight * 8 / g);

			// 计算跳到特定高度垂直方向需要的速度
            float vSpeed = totalTime * g / 2;
			// 计算跳到前方特定距离需要的向前的速度
            float fSpeed = jumpDistance / totalTime;


            // 用计算好的速度发射小球
            playerRigidbody.velocity = new Vector3(0, vSpeed, fSpeed);
        }
        if (col.tag == "Pick Up")
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.gems);
            // 碰撞检测到了钻石
            col.gameObject.SetActive(false);

            // 增加钻石数量
            int numberOfPickUps = PlayerPrefs.GetInt("NumberOfPickUps");
            numberOfPickUps++;
            PlayerPrefs.SetInt("NumberOfPickUps", numberOfPickUps);

            // 刷新钻石数
            uiController.updateNumberOfGemsUITexts();

            // 实例化钻石爆炸特效
            GameObject gemExplosionObject = Instantiate(gemsExplosion, new Vector3(transform.position.x, gemsExplosion.transform.position.y, transform.position.z), gemsExplosion.transform.rotation) as GameObject;

            Destroy(gemExplosionObject, 1f);
        }
    }

    void controlBallRotation ()
	{
		if (gameStarted && !gameOver)
			// 绕着X轴向旋转小球
			transform.Rotate(Vector3.right * 500 *Time.deltaTime, Space.World);
	}

	void controlGravity ()
	{
		// 计算新的重力
		float newYGravity = Physics.gravity.y - gravityStep;

		// 每次得分后增加重力直到变为最大重力
		if (score >0 && score % gravityRate == 0 && newYGravity > maxGravity)
		{
			Physics.gravity = new Vector3(0, newYGravity, 0);
		}
	}

    void initCurBall()
    {
        GameObject curBall = null;
        string curBallName = PlayerPrefs.GetString("CurBall", "Ball1");
        for (int i = 0; i < ballsAry.Length; ++i)
        {
            if (ballsAry[i].name.Equals(curBallName))
            {
                curBall = ballsAry[i];
                break;
            }
        }

        Material curBallMat = curBall.GetComponent<MeshRenderer>().sharedMaterial;
        gameObject.GetComponent<MeshRenderer>().material = curBallMat;
    }

    private void OnSpeedFactorChanged(float value)
    {
        if (value!= speedFactor)
        {
            PlayerPrefs.SetFloat("SpeedFactor", value);
        }
    }


    private void OnDisable()
    {
        speedFactorSlider.onValueChanged.RemoveAllListeners();
    }
    #endregion
}
