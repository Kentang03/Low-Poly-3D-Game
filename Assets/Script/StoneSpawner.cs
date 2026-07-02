using UnityEngine;

public class StoneSpawner : MonoBehaviour
{
    [Header("Stone Settings")]
    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float throwAngle = 45f;
    [SerializeField] private Vector3 throwDirection = Vector3.forward;
    
    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private bool autoSpawn = false;
    [SerializeField] private int maxStones = 10;
    [SerializeField] private float stoneLifetime = 10f;
    
    [Header("Physics Settings")]
    [SerializeField] private float mass = 1f;
    [SerializeField] private float drag = 0.1f;
    [SerializeField] private float angularDrag = 0.05f;
    
    [Header("Visual Helpers")]
    [SerializeField] private bool showTrajectory = true;
    [SerializeField] private int trajectoryPoints = 30;
    [SerializeField] private float trajectoryTimeStep = 0.1f;
    [SerializeField] private Color trajectoryColor = Color.red;
    
    private float lastSpawnTime;
    private int currentStoneCount;

    void Start()
    {
        if (spawnPoint == null)
            spawnPoint = transform;
    }

    void Update()
    {
        if (autoSpawn && Time.time - lastSpawnTime >= spawnInterval)
        {
            SpawnStone();
            lastSpawnTime = Time.time;
        }
        
        // Manual spawn dengan tombol Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnStone();
        }
    }

    public void SpawnStone()
    {
        if (stonePrefab == null)
        {
            Debug.LogError("Stone prefab tidak diatur!");
            return;
        }

        if (currentStoneCount >= maxStones)
        {
            Debug.Log("Maksimal batu tercapai: " + maxStones);
            return;
        }

        // Spawn batu di posisi spawn point
        GameObject newStone = Instantiate(stonePrefab, spawnPoint.position, spawnPoint.rotation);
        
        // Pastikan ada Rigidbody
        Rigidbody rb = newStone.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = newStone.AddComponent<Rigidbody>();
        }

        // Set physics properties
        rb.mass = mass;
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;
        
        // Add collision handler for player damage
        RockCollisionHandler collisionHandler = newStone.GetComponent<RockCollisionHandler>();
        if (collisionHandler == null)
        {
            collisionHandler = newStone.AddComponent<RockCollisionHandler>();
            Debug.Log($"Added RockCollisionHandler to {newStone.name}");
        }

        // Hitung velocity untuk gerakan parabola
        Vector3 velocity = CalculateTrajectoryVelocity();
        rb.linearVelocity = velocity;
        
        // Add random spin untuk efek lebih realistis
        rb.angularVelocity = new Vector3(
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f)
        );

        currentStoneCount++;
        
        // Hapus batu setelah waktu yang ditentukan untuk menghemat memory
        Destroy(newStone, stoneLifetime);
        
        // Kurangi counter ketika batu dihapus
        StartCoroutine(DecreaseStoneCountAfterDelay(stoneLifetime));
    }

    private Vector3 CalculateTrajectoryVelocity()
    {
        // Normalisasi arah throw
        Vector3 direction = throwDirection.normalized;
        
        // Konversi angle ke radian
        float angleRad = throwAngle * Mathf.Deg2Rad;
        
        // Hitung komponen horizontal dan vertikal
        float horizontalForce = throwForce * Mathf.Cos(angleRad);
        float verticalForce = throwForce * Mathf.Sin(angleRad);
        
        // Buat velocity vector
        Vector3 velocity = direction * horizontalForce + Vector3.up * verticalForce;
        
        return velocity;
    }

    private System.Collections.IEnumerator DecreaseStoneCountAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentStoneCount = Mathf.Max(0, currentStoneCount - 1);
    }

    void OnDrawGizmos()
    {
        if (!showTrajectory || spawnPoint == null)
            return;

        // Gambar trajectory path
        Vector3 velocity = CalculateTrajectoryVelocity();
        Vector3 position = spawnPoint.position;
        Vector3 previousPosition = position;

        Gizmos.color = trajectoryColor;
        
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float time = i * trajectoryTimeStep;
            
            // Hitung posisi berdasarkan physics trajectory
            Vector3 displacement = velocity * time + 0.5f * Physics.gravity * time * time;
            Vector3 drawPosition = spawnPoint.position + displacement;
            
            if (i > 0)
            {
                Gizmos.DrawLine(previousPosition, drawPosition);
            }
            
            previousPosition = drawPosition;
            
            // Stop jika trajectory sudah di bawah ground (y < 0 misalnya)
            if (drawPosition.y < spawnPoint.position.y - 10f)
                break;
        }
        
        // Gambar arah throw
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(spawnPoint.position, throwDirection.normalized * 3f);
        
        // Gambar spawn point
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        // Gambar additional info ketika object dipilih
        if (spawnPoint == null)
            spawnPoint = transform;
            
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(spawnPoint.position + throwDirection.normalized * 2f, Vector3.one * 0.2f);
    }

    // Public methods untuk diakses dari script lain atau UI
    public void SetThrowForce(float force)
    {
        throwForce = Mathf.Max(0, force);
    }

    public void SetThrowAngle(float angle)
    {
        throwAngle = Mathf.Clamp(angle, 0f, 90f);
    }

    public void SetThrowDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
            throwDirection = direction.normalized;
    }

    public void SetStoneLifetime(float lifetime)
    {
        stoneLifetime = Mathf.Max(0.1f, lifetime);
    }

    public void ToggleAutoSpawn()
    {
        autoSpawn = !autoSpawn;
    }

    // Method untuk mengatur arah berdasarkan target
    public void AimAtTarget(Transform target)
    {
        if (target != null && spawnPoint != null)
        {
            Vector3 direction = (target.position - spawnPoint.position).normalized;
            direction.y = 0; // Hanya arah horizontal
            throwDirection = direction;
        }
    }
}