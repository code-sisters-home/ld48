using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	[SerializeField] private Animator _animator;

	public float life = 10;
	private bool isPlat;
	private bool isObstacle;
	private Transform fallCheck;
	private Transform wallCheck;
	public LayerMask turnLayerMask;
	private Rigidbody2D rb;

	private bool facingRight = true;
	
	public float speed = 5f;

	public bool isInvincible = false;
	private bool isHitted = false;
	public bool isThrowingObjects = false;
	public float weaponShiftY = 0f;
	public GameObject throwableObject;	
	public GameObject player;

    private float nextThrowTime = 0.0f;
    public float throwPeriod = 3.0f;

	void Awake () {
		fallCheck = transform.Find("FallCheck");
		wallCheck = transform.Find("WallCheck");
		rb = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void FixedUpdate () {

		if (life <= 0) {
			_animator.SetBool("IsDead", true);
			StartCoroutine(DestroyEnemy());
		}

		isPlat = Physics2D.OverlapCircle(fallCheck.position, .2f, 1 << LayerMask.NameToLayer("Default"));
		isObstacle = Physics2D.OverlapCircle(wallCheck.position, .2f, turnLayerMask);

		if (!isHitted && life > 0 && Mathf.Abs(rb.velocity.y) < 0.5f)
		{
            if (isPlat && !isObstacle && !isHitted)
            {
                if (isThrowingObjects)
                {
                    Vector2 playerPos = new Vector2(player.transform.localPosition.x, player.transform.localPosition.y);
					Vector2 weaponShift = new Vector2(0, weaponShiftY);
                    Vector2 direction = (playerPos - weaponShift - new Vector2(transform.localPosition.x, transform.localPosition.y)).normalized;
                    if ((facingRight && direction.x < 0 || !facingRight && direction.x > 0 )&& Time.time > nextThrowTime)
                    {
						nextThrowTime = Time.time + throwPeriod;
                        throwObject(direction, new Vector3(weaponShift.x, weaponShift.y, 0));
						
                    }
                }
                if (facingRight)
                {
					rb.velocity = new Vector2(-speed, rb.velocity.y);
				}
				else
				{
					rb.velocity = new Vector2(speed, rb.velocity.y);
				}
			}
			else
			{
				Flip();
			}
		}
	}

	void Flip (){
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public void ApplyDamage(float damage) {
		if (!isInvincible) 
		{
			float direction = damage / Mathf.Abs(damage);
			damage = Mathf.Abs(damage);
			_animator.SetBool("Hit", true);
			life -= damage;
			rb.velocity = Vector2.zero;
			rb.AddForce(new Vector2(direction * 500f, 100f));
			StartCoroutine(HitTime());
		}
	}

	void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Player" && life > 0)
		{
			collision.gameObject.GetComponent<CharacterController2D>().ApplyDamage(2f, transform.position);
		}
		if (collision.gameObject.tag != "Player")
		{
			gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2( transform.position.x * (-1), transform.position.y * (-1)));
		}
	}

	IEnumerator HitTime()
	{
		isHitted = true;
		isInvincible = true;
		yield return new WaitForSeconds(0.1f);
		isHitted = false;
		isInvincible = false;
	}

	IEnumerator DestroyEnemy()
	{
		CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
		capsule.size = new Vector2(1f, 0.25f);
		capsule.offset = new Vector2(0f, -0.8f);
		capsule.direction = CapsuleDirection2D.Horizontal;
		yield return new WaitForSeconds(0.25f);
		rb.velocity = new Vector2(0, rb.velocity.y);
		yield return new WaitForSeconds(3f);
		Destroy(gameObject);
	}
	void throwObject(Vector2 direction, Vector3 weaponShift)
	{
		if(!isThrowingObjects)
		  return;
		GameObject throwableWeapon = Instantiate(throwableObject, transform.position + weaponShift + new Vector3(transform.localScale.x * 0.5f,-0.2f), Quaternion.identity) as GameObject;

        if (direction.x > 0)
        {
            Vector3 theScale = throwableWeapon.transform.localScale;
            theScale.x *= -1;
            throwableWeapon.transform.localScale = theScale;
        }
		throwableWeapon.GetComponent<ThrowableWeapon>().direction = direction; 
		throwableWeapon.transform.Rotate(new Vector3(0, 0, 1), Mathf.Sign(direction.x)*Mathf.Asin(direction.y)*Mathf.Rad2Deg);
		throwableWeapon.name = "ThrowableWeapon";
	}	
}
