using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public PlayerDeadController PlayerDeadController;
    //どのくらい飛ばし率を追加するか
    public float AddBurstRate = 5.5f;
    // 力の強さ
    public float forceAmount = 10f; 
    //飛ばす方向の指定
    public bool AddForceRight = false;
    //監督
    public CameraShake CameraShake;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && PlayerDeadController.HitEnemy == false)
        {

            // プレイヤーの位置と敵の位置を取得
            Vector3 playerPosition = collision.transform.position;
            Vector3 enemyPosition = transform.position;

            // プレイヤーが敵の上を踏んだか横から当たったかを判定
            if (IsAboveEnemy(playerPosition, enemyPosition, collision))
            {
                //飛ばし率の減少
                if(PlayerDeadController.BurstRate-AddBurstRate/2<=0)
                {
                    PlayerDeadController.BurstRate = 0;
                }
                else
                {
                    PlayerDeadController.BurstRate -= AddBurstRate/2;
                }
                
                // プレイヤーが敵の上を踏んだ場合: 敵を倒す
                //SE
                PlayerDeadController.PlayKnockEnemySE();
                // ヒットストップ処理挿入
                HitStopController.instance.StartHitStop(0.3f);
                Destroy(gameObject); 
            }
            else //横から当たった時
            {
                //飛ばし率の追加
                PlayerDeadController.BurstRate += AddBurstRate;
                //操作不能
                PlayerDeadController.HitEnemy = true;
                //SE
                PlayerDeadController.PlayHitSE();
                //カメラ揺らす
                StartCoroutine(CameraShake.Shake(0.3f, 0.2f));
                // ヒットストップ処理挿入
                HitStopController.instance.StartHitStop(0.7f);

                // プレイヤーのRigidbody2Dを取得
                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (AddForceRight == false)
                {
                    // 左方向に力を加える（x軸負方向）
                    rb.AddForce(Vector2.left * forceAmount*PlayerDeadController.BurstRate, ForceMode2D.Impulse);
                }
                else
                {
                    //右側に力を加える
                    rb.AddForce(Vector2.right * forceAmount*PlayerDeadController.BurstRate, ForceMode2D.Impulse);
                }
            }
            
        }
    }

    // プレイヤーが敵の上に乗っているかを判定するメソッド
    private bool IsAboveEnemy(Vector3 playerPosition, Vector3 enemyPosition, Collision2D collision)
    {
        
        //プレイヤーの当たり判定の大きさ取得
        Vector2 colliderSize = collision.gameObject.GetComponent<CapsuleCollider2D>().size;
        // プレイヤーが敵の上にいるかかつ速度が負の方向かどうかを判定
        if (playerPosition.y > enemyPosition.y+colliderSize.y*collision.gameObject.transform.localScale.y/2 && collision.gameObject.GetComponent<Rigidbody2D>().velocity.y <= 0)
        {
            return true;
        }

        return false;
    }
}