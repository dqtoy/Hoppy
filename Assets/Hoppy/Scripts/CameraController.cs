using UnityEngine;
using System.Collections;


public class CameraController : MonoBehaviour {


	#region 变量定义

	// player对象引用
	public GameObject player;

	// 相机和Player的距离
	private Vector3 offset;
	// 相机位置在x轴向的限制
	private float xPosLimit = 1.5f;

	#endregion



	#region Unity回调

	void Start ()
	{
		// 计算相机和player的距离
		offset = transform.position - player.transform.position;
	}
	
	void LateUpdate ()
	{
        // 相机只在x和z轴向移动
        // 而且x轴线被限制在xPosLimit和-xPosLimit之间
        if (player != null)
			transform.position = new Vector3 (Mathf.Clamp(player.transform.position.x + offset.x, -xPosLimit, xPosLimit), transform.position.y, player.transform.position.z + offset.z);
		
	}

	#endregion
}
