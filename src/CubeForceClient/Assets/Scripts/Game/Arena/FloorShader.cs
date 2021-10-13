using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorShader : MonoBehaviour
{
    public ComputeShader shader;
    public int tex_resolution;
    public int diamond_count_dim;
    public int diamond_width;
    public float frequency;
    public float amplitude;
    public float speed;
    public bool use_noise;

    public Color background;

    private Renderer renderer;
    private RenderTexture render_texture;
    private float step;
    private int kernel_id;

    private ComputeBuffer buffer_diamonds;

    // Start is called before the first frame update
    void Start()
    {
        render_texture = new RenderTexture(tex_resolution,tex_resolution,24);
        render_texture.enableRandomWrite = true;
        render_texture.Create();

        renderer = GetComponent<Renderer>();
        renderer.enabled = true;


        int[] diamond_color_ids =  new int[] {
                                                1,
                                                2,
                                                3
                                            };

        int[] diamonds = generateTexture(tex_resolution, diamond_count_dim, diamond_width,diamond_color_ids);
        Texture2D perlin_noise = perlinNoise(tex_resolution);
        perlin_noise.Apply();
        
        
        buffer_diamonds = new ComputeBuffer(diamonds.Length, 4);
        buffer_diamonds.SetData(diamonds);
        
        kernel_id = shader.FindKernel("CSMain");

        shader.SetBuffer(kernel_id, "diamonds", buffer_diamonds);
        shader.SetFloat("background_r",background.r);
        shader.SetFloat("background_g",background.g);
        shader.SetFloat("background_b",background.b);
        shader.SetFloat("res",tex_resolution);
        shader.SetFloat("frequency",frequency);
        shader.SetFloat("amplitude",amplitude);
        shader.SetBool("use_noise",use_noise);
        shader.SetTexture(kernel_id, "noise", perlin_noise);
        shader.SetTexture(kernel_id, "Result", render_texture);
        //shader.SetTexture(kernel_id, "diamonds" , diamonds);
        renderer.material.SetTexture("_BaseMap",render_texture);
        shader.Dispatch(kernel_id, tex_resolution/8,tex_resolution/8,1);
    }


    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown("a")){
            
        //}
    }

    void FixedUpdate(){
        step = (step + speed) % tex_resolution;
        shader.SetFloat("step",step);
        shader.SetFloat("frequency",frequency);
        shader.SetFloat("amplitude",amplitude);
        shader.SetFloat("background_r",background.r);
        shader.SetFloat("background_g",background.g);
        shader.SetFloat("background_b",background.b);
        shader.SetBool("use_noise",use_noise);
        shader.Dispatch(kernel_id, tex_resolution/8,tex_resolution/8,1);
    }

    private int[] generateTexture(int res, int diamond_count, int diamond_width, int[] diamond_color_ids){
        int[,] tex2d = new int[res,res];
        //return new int[res*res];

        int s = (int)(tex_resolution/diamond_count);
        int color_index_start = 0;

        for(int x = 0; x < tex_resolution; x++)
        {
            int color_index = color_index_start;

            for(int y = 0; y < tex_resolution; y++){
                if(x % s == 0 && y % s == 0){
                    tex2d[x,y] = diamond_color_ids[color_index];
                    color_index = (color_index + 1) % diamond_color_ids.Length;
                }
            }

            color_index_start = (color_index_start + 1) % diamond_color_ids.Length; 
        }

        for(int i = 0; i < diamond_width/2; i++){
            tex2d = morph(tex2d,res);
        }

        int[] tex = new int[res * res];
        for(int x = 0; x < res; x++){
            for(int y = 0; y < res; y++){
                tex[y + x * res] = tex2d[x,y];
            }
        }
        return tex;
    }

    private int[,] morph(int[,] tex, int res){
        int[,] tex_morphed = new int[res,res];
        for(int x = 0; x < res; x++){
            for(int y = 0; y < res; y++){
                if(tex[x,y] != 0){
                    tex_morphed[x,y] = tex[x,y];
                    tex_morphed[Mathf.Max(x-1,0),y] = tex[x,y];
                    tex_morphed[x,Mathf.Max(y-1,0)] = tex[x,y];
                    tex_morphed[Mathf.Min(x+1,res-1),y] = tex[x,y];
                    tex_morphed[x,Mathf.Min(y+1,res-1)] = tex[x,y];
                }
            }
        }
        return tex_morphed;
    }

    private Texture2D perlinNoise(int res){
        Texture2D perlin_noise = new Texture2D(res,res);

        float scale = 32;
        for(int x = 0; x < res; x++){
            for(int y = 0; y < res; y++){
                float sample = Mathf.PerlinNoise((float)x/res * scale, (float)y/res * scale);
                float sample_scaled = sample * 0.8f + 0.2f;
                perlin_noise.SetPixel(x,y,new Color(sample_scaled,sample_scaled,sample_scaled));
            }
        }

        return perlin_noise;
    }

    void OnDisable () {
		buffer_diamonds.Release();
	}
}
