using UnityEngine;

public class Runner : MonoBehaviour
{
    public Animator runner_anim;
    public int left_counter = 0;
    public float go = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray_left = new Ray(transform.position, -transform.right);
        Ray ray_forward = new Ray(transform.position, transform.forward);
        Ray ray_right = new Ray(transform.position, transform.right);
        
        RaycastHit hit_left;
        RaycastHit hit_forward;
        RaycastHit hit_right;

        if(Physics.Raycast(ray_forward, out hit_forward, 1.5f))
        {
            if(hit_forward.transform.gameObject.tag == "Wall")
            {
                if(Physics.Raycast(ray_left, out hit_left, 3f))
                {
                    if(hit_left.transform.gameObject.tag == "Wall")
                    {
                        if(Physics.Raycast(ray_right, out hit_right, 3f))
                        {
                            if(hit_right.transform.gameObject.tag == "Wall")
                            {
                                transform.Rotate(Vector3.up * 180);
                            }
                        }
                        else
                        {
                            transform.Rotate(Vector3.up * 90);
                        }
                    }
                }
                else
                {
                    transform.Rotate(Vector3.up * -90); 
                }
            }
        }
        else
        {
            transform.Translate(Vector3.forward * go * Time.deltaTime);            
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
        Gizmos.DrawRay(transform.position, -transform.right * 2f);
        Gizmos.DrawRay(transform.position, transform.right * 2f);
    }
}
