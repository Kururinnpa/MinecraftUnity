using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Rigidbody rigid;
    private CapsuleCollider capsuleCollider;
    private GameObject cameraObj;
    private new Camera camera;
    public GameObject inventoryObj;
    private Inventory inventory;

    private float lookSpeedX = 2f;           // 鼠标水平移动速度
    private float lookSpeedY = 2f;           // 鼠标垂直移动速度
    private float moveSpeed = 5f;            // 移动速度
    private float rotationX = 0f;            // 当前垂直旋转角度
    private float jumpHeight = 1.5f;         // 跳跃高度
    private const float gravity = -9.8f * 3; // 重力
    private bool isGrounded;
    private BlockType buildType = BlockType.AIR;
    private int curBuildSlotIndex = 0;
    public Slot curBuildSlot;
    public float scrollSensitivity = 6.0f;

    private Vector2Int defaultChunkPos = new Vector2Int(int.MaxValue, int.MaxValue);
    private Vector3Int defaultHitPos = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);

    private Vector2Int preChunkPos;
    private Vector3Int preHitPos;

    private float attack = 20f;               // 每秒5点伤害

    // Start is called before the first frame update
    void Start()
    {
        isGrounded = IsGrounded();
        inventoryObj = GameObject.Find("Inventory");
        inventory = inventoryObj.GetComponent<Inventory>();
        inventory.InitInventory(this);
        curBuildSlot = inventory.hotbarSlots[0];
        curBuildSlot.selection.gameObject.SetActive(true);
        inventoryObj.SetActive(false);

        preChunkPos = defaultChunkPos;
        preHitPos = defaultHitPos;

        // 设置刚体
        rigid = gameObject.AddComponent<Rigidbody>();
        rigid.useGravity = false;
        rigid.freezeRotation = true;
        rigid.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // 设置碰撞体
        capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.height = 2;
        capsuleCollider.radius = 0.25f;
        capsuleCollider.center = new Vector3(0, 0, 0);

        // 设置摩擦力为0，防止卡墙
        PhysicMaterial noFrictionMaterial = new PhysicMaterial();
        noFrictionMaterial.dynamicFriction = 0f;
        noFrictionMaterial.staticFriction = 0f;
        noFrictionMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        capsuleCollider.material = noFrictionMaterial;

        // 设置相机
        cameraObj = new GameObject("Camera");
        cameraObj.transform.position = transform.position + new Vector3(0, 1, 0);
        camera = cameraObj.AddComponent<Camera>();
        camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
        camera.fieldOfView = 90;
        camera.nearClipPlane = 0.01f;

        // 设置光标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        inventory.ChangeState();

        if (!inventoryObj.activeSelf)
        {
            UpdateCamera();
            UpdatePlayer();
            BlockInteraction();
            ChangeBuildSlot();
        }
        else
        {
        }
    }

    private void FixedUpdate()
    {
        if (!IsGrounded())
            rigid.AddForce(gravity * Vector3.up, ForceMode.Acceleration);
    }

    private void UpdateCamera()
    {
        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 计算旋转角度
        rotationX -= mouseY * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);  // 限制垂直旋转角度

        // 应用旋转
        cameraObj.transform.rotation = Quaternion.Euler(rotationX, cameraObj.transform.rotation.eulerAngles.y + mouseX * lookSpeedX, 0);
    }

    private void UpdatePlayer()
    {
        isGrounded = IsGrounded();

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 horizontalDirection = Vector3.Normalize(new Vector3(cameraObj.transform.right.x, 0, cameraObj.transform.right.z));
        Vector3 verticalDirection = Vector3.Normalize(new Vector3(cameraObj.transform.forward.x, 0, cameraObj.transform.forward.z));
        Vector3 moveDirection = horizontalDirection * horizontal + verticalDirection * vertical;

        if (isGrounded && (horizontal != 0 || vertical != 0))
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection); // 计算目标旋转

            // 当前旋转逐渐过渡到目标旋转
            float rotationSpeed = 720f; // 旋转速度
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        cameraObj.transform.position = transform.position + new Vector3(0, 1, 0);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rigid.velocity = new Vector3(rigid.velocity.x, Mathf.Sqrt(jumpHeight * -2f * gravity), rigid.velocity.z);
        }

        // 处理水平移动
        Vector3 targetVelocity = moveDirection * moveSpeed;
        targetVelocity.y = rigid.velocity.y;  // 保持跳跃时的垂直速度
        rigid.velocity = targetVelocity;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    private void BlockInteraction()
    {
        Debug.DrawRay(cameraObj.transform.position, cameraObj.transform.forward * 4, Color.red);

        int chunkLayer = LayerMask.NameToLayer("Chunk");
        int layerMask = 1 << chunkLayer;

        if (Input.GetMouseButtonDown(1))
        {
            if(buildType == BlockType.AIR)
            {
                return;
            }

            if (Physics.Raycast(cameraObj.transform.position, cameraObj.transform.forward, out RaycastHit hit, 4, layerMask))
            {
                Vector3 pos = hit.point - hit.normal / 2; // 立方体内部的位置
                // 在一个区块中的位置
                int rltX = (int)(Mathf.Round(pos.x) - hit.collider.gameObject.transform.position.x);
                int rltY = (int)(Mathf.Round(pos.y) - hit.collider.gameObject.transform.position.y);
                int rltZ = (int)(Mathf.Round(pos.z) - hit.collider.gameObject.transform.position.z);
                Vector3Int buildPos = new Vector3Int(rltX, rltY, rltZ); // 修改的位置

                if (hit.normal == Vector3.up)
                {
                    buildPos.y += 1;
                }
                else if (hit.normal == Vector3.down)
                {
                    buildPos.y -= 1;
                }
                else if (hit.normal == Vector3.right)
                {
                    buildPos.x += 1;
                }
                else if (hit.normal == Vector3.left)
                {
                    buildPos.x -= 1;
                }
                else if (hit.normal == Vector3.forward)
                {
                    buildPos.z += 1;
                }
                else if (hit.normal == Vector3.back)
                {
                    buildPos.z -= 1;
                }

                // 所在区块的位置
                pos.x = Mathf.Floor(pos.x + 0.5f);
                pos.y = Mathf.Floor(pos.y);
                pos.z = Mathf.Floor(pos.z + 0.5f);
                int chunkX = Mathf.FloorToInt(pos.x / Chunk.chunkSize);
                int chunkZ = Mathf.FloorToInt(pos.z / Chunk.chunkSize);
                if (buildPos.x < 0)
                {
                    chunkX -= 1;
                    buildPos.x = Chunk.chunkSize - 1;
                }
                else if (buildPos.x >= Chunk.chunkSize)
                {
                    chunkX += 1;
                    buildPos.x = 0;
                }

                if (buildPos.z < 0)
                {
                    chunkZ -= 1;
                    buildPos.z = Chunk.chunkSize - 1;
                }
                else if (buildPos.z >= Chunk.chunkSize)
                {
                    chunkZ += 1;
                    buildPos.z = 0;
                }

                World world = GameObject.Find("World").GetComponent<World>();
                Chunk chunk = world.GetChunk(chunkX, chunkZ);
                chunk.GetBlock(buildPos.x, buildPos.y, buildPos.z).SetBlockType(buildType);
                chunk.GetBlock(buildPos.x, buildPos.y, buildPos.z).Recover();

                Vector3 absPos = buildPos + new Vector3Int(chunkX * Chunk.chunkSize, 0, chunkZ * Chunk.chunkSize);
                //Debug.Log("build pos: " + absPos);
                //Debug.Log("if has block: " + world.IsDropBlockAtPosition(absPos));
                if (world.IsDropBlockAtPosition(absPos))
                {
                    DropBlock dropBlock = world.GetDropBlockAtPosition(absPos);
                    dropBlock.MoveBlock();
                }

                curBuildSlot.BuildOneBlock();
                RefreshBuildType();
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(cameraObj.transform.position, cameraObj.transform.forward, out RaycastHit hit, 4, layerMask))
            {
                Vector3 pos = hit.point - hit.normal / 2; // 立方体内部的位置
                // 在一个区块中的位置
                int rltX = (int)(Mathf.Round(pos.x) - hit.collider.gameObject.transform.position.x);
                int rltY = (int)(Mathf.Round(pos.y) - hit.collider.gameObject.transform.position.y);
                int rltZ = (int)(Mathf.Round(pos.z) - hit.collider.gameObject.transform.position.z);
                Vector3Int hitPos = new Vector3Int(rltX, rltY, rltZ); // 修改的位置

                // 所在区块的位置
                pos.x = Mathf.Floor(pos.x + 0.5f);
                pos.y = Mathf.Floor(pos.y);
                pos.z = Mathf.Floor(pos.z + 0.5f);
                int chunkX = Mathf.FloorToInt(pos.x / Chunk.chunkSize);
                int chunkZ = Mathf.FloorToInt(pos.z / Chunk.chunkSize);
                Vector2Int chunkPos = new Vector2Int(chunkX, chunkZ);

                if (chunkPos != preChunkPos || hitPos != preHitPos) // 在一个方块销毁之前移走，生命值回满
                {
                    if (preChunkPos != defaultChunkPos && preHitPos != defaultHitPos)
                    {
                        Chunk preChunk = GameObject.Find("World").GetComponent<World>().GetChunk(preChunkPos.x, preChunkPos.y);
                        preChunk.GetBlock(preHitPos.x, preHitPos.y, preHitPos.z).Recover();
                    }

                    preHitPos = hitPos;
                    preChunkPos = chunkPos;
                }

                Chunk chunk = GameObject.Find("World").GetComponent<World>().GetChunk(chunkX, chunkZ);
                //chunk.HitBlock(hitPos);
                chunk.GetBlock(hitPos.x, hitPos.y, hitPos.z).GetHit(Time.deltaTime * attack);
            }
            else
            {
                if (preChunkPos != defaultChunkPos && preHitPos != defaultHitPos)
                {
                    Chunk preChunk = GameObject.Find("World").GetComponent<World>().GetChunk(preChunkPos.x, preChunkPos.y);
                    preChunk.GetBlock(preHitPos.x, preHitPos.y, preHitPos.z).Recover();
                    preChunkPos = defaultChunkPos;
                    preHitPos = defaultHitPos;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (preChunkPos != defaultChunkPos && preHitPos != defaultHitPos)
            {
                Chunk preChunk = GameObject.Find("World").GetComponent<World>().GetChunk(preChunkPos.x, preChunkPos.y);
                preChunk.GetBlock(preHitPos.x, preHitPos.y, preHitPos.z).Recover();
            }
        }
    }

    public void AddItemToInventory(Item item)
    {
        inventory.AddItem(item);
    }

    public void RefreshBuildType()
    {
        if (curBuildSlot.itemInSlot.item != null)
            buildType = curBuildSlot.itemInSlot.item.bType;
        else
            buildType = BlockType.AIR;
    }

    public void ChangeBuildSlot()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            // 调整滚轮的灵敏度
            int scrollChange = Mathf.RoundToInt(scrollInput * scrollSensitivity);
            int preIndex = curBuildSlotIndex;

            curBuildSlotIndex -= scrollChange;

            if (curBuildSlotIndex > 8) // 如果超过了8，则回到0
            {
                curBuildSlotIndex = 0;
            }
            else if (curBuildSlotIndex < 0) // 如果小于0，则回到8
            {
                curBuildSlotIndex = 8;
            }

            // 更新当前选中的Slot
            curBuildSlot.selection.gameObject.SetActive(false);
            curBuildSlot = inventory.hotbarSlots[curBuildSlotIndex];
            curBuildSlot.selection.gameObject.SetActive(true);

            if (curBuildSlot.itemInSlot.item != null)
                buildType = curBuildSlot.itemInSlot.item.bType;
            else
                buildType = BlockType.AIR;
        }
    }
}
