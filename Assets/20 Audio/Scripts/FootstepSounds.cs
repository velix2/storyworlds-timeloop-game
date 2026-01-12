using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = System.Random;

public class FootstepSounds : MonoBehaviour
{
    private static bool staticsInitiated;

    private static Sound wetStep;
    private static Sound woodStep;
    private static Sound snowStep;
    private static Sound tileStep;

    private AudioSource source1;
    private AudioSource source2;
    private bool sourceAlternation;
    private readonly float pitchVariance = 0.05f;

    private static LayerMask groundMask;
    private TerrainTextureDetector terrainTextureDetector;
    
    private void Start()
    {
        source1 = gameObject.AddComponent<AudioSource>();
        source1.outputAudioMixerGroup = AudioManager.Instance.StepGroup;
        
        source2 = gameObject.AddComponent<AudioSource>();
        source2.outputAudioMixerGroup = AudioManager.Instance.StepGroup;
        UpdateTerrainTextureDetector();
        
        if (staticsInitiated) return;
        wetStep = Resources.Load<Sound>("Sounds/Step Wet");
        woodStep = Resources.Load<Sound>("Sounds/Step Wood");
        snowStep = Resources.Load<Sound>("Sounds/Step Snow");
        tileStep = Resources.Load<Sound>("Sounds/Step Tile");

        groundMask = LayerMask.GetMask("Ground");
        
        staticsInitiated = true;
    }

    private float lastStep = 0;
    private float stepCooldown = 0.28f;
    public void PlayFootstep()
    {
        //very scuffed solution for the following problem:
        //BlendTrees in AnimationControllers play several Animations at the same time, animation events therefore trigger multiple times
        //to avoid this I have added a stepCooldown which is approximately the time between each footstep in each walking animation
        if (Time.time < lastStep + stepCooldown) return;
        lastStep = Time.time;
        
        GroundData.groundType type = GroundData.groundType.DEFAULT;
        
        if (Physics.Raycast(transform.position, -transform.up, out var hit, 3, groundMask))
        {
            GroundData ground = hit.transform.gameObject.GetComponent<GroundData>();
            if (ground != null)
            {
                type = ground.type;
            }
            else
            {
                int terrainIndex = terrainTextureDetector.GetDominantTextureIndexAt(hit.transform.position);
                type = TerrainToGroundType(terrainIndex);
            }

        }
        
        AudioClip clip = null;
        switch (type)
        {
            case GroundData.groundType.WOOD:
                if(woodStep != null)
                    clip = woodStep.GetAudioClip();
                break;
            case GroundData.groundType.WET:
                if (wetStep != null)
                    clip = wetStep.GetAudioClip();
                break;
            case GroundData.groundType.SNOW:
                if (snowStep != null)
                    clip = snowStep.GetAudioClip();
                break;
            case GroundData.groundType.TILE:
            default:
                if (tileStep != null)
                    clip = tileStep.GetAudioClip();
                break;
        }



        AudioSource source = sourceAlternation ? source1 : source2;
        sourceAlternation = !sourceAlternation;
        if(source != null && clip != null)
        {
            source.pitch = UnityEngine.Random.Range(1.0f - pitchVariance, 1.0f + pitchVariance);
            source.clip = clip;
            source.Play();
        }
    }

    private GroundData.groundType TerrainToGroundType(int textureIndex)
    {
        //default to snow when on terrain
        return GroundData.groundType.SNOW;
    }
    
    
    public void UpdateTerrainTextureDetector()
    {
        if (Terrain.activeTerrain == null) return;
        terrainTextureDetector = Terrain.activeTerrain.gameObject.GetComponent<TerrainTextureDetector>();
        if (terrainTextureDetector == null) Debug.LogError("There is no TerrainTextureDetector on the Terrain GameObject in this Scene.");
    }
}
