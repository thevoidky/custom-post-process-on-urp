# Custom Post-process on URP

Support shader-based customized post-process features on Universal Render Pipeline.

## Table of Contents
- [Requirements](#requirements)
- [Limitation](#limitation)
- [Installation](#installation)
- [Sample](#sample)

## Requirements

- UniTask (https://github.com/Cysharp/UniTask)
- Tested on Unity 2020.3.6f1

## Limitation

- It is suppoted on ```ForwardRenderer``` only, not on ```Renderer2D``` and others.
- Only the color texture is supported to shaders via ```_MainTex``` property.

## Installation

It is supported via Package Manager with git URL only.

![01](https://user-images.githubusercontent.com/22534449/131255940-c0896eb0-5a76-4d7e-93f2-77d83841a900.png)

Input ```https://github.com/thevoidky/custom-post-process-on-urp.git#experimental``` and ```Add```.

![02](https://user-images.githubusercontent.com/22534449/131256532-fe0212f2-4d5b-48d5-b66b-5bc55d227ab5.png)

## Sample

![03](https://user-images.githubusercontent.com/22534449/131256592-8ed9da86-f2bf-4bea-891a-9d06f4e13881.png)

If you succeeded to install the package, you could see the sample,

![image](https://user-images.githubusercontent.com/22534449/131362513-7bc65780-d60d-4707-a01d-b58dc09eb721.png)

and as to import, you could see a sample scene. This sample is preset of an URP project that is applied custom post-process.

![image](https://user-images.githubusercontent.com/22534449/131256909-c7a83919-60dd-4b53-85c3-9b87527fcc83.png)
![image](https://user-images.githubusercontent.com/22534449/131256927-4900fcfa-b599-49ee-9f7a-aaece2289965.png)

You could see a ```CustomPostProcessSource``` component at ```Main Camera``` GameObject and a ```CustomPostProcessComponent``` component at ```CustomPostProcessComponent``` GameObject.

Let's play the scene and click "Toggle to on" of the ```CustomPostProcessAdapter```.
The scene would be grayscaled as ```Toggle to on``` clicked, and would be back to original as ```Toggle to off``` clicked.

![1630333590482](https://user-images.githubusercontent.com/22534449/131354983-2a671da7-ad2d-446b-8655-d34fe8e917bf.gif)

Renderer features you want to apply as post-process have to added to ```ForwardRendererData``` used by the camera. Let's see that.

![image](https://user-images.githubusercontent.com/22534449/131365877-06c694f8-507f-4ac0-bf5f-787c2077f768.png)

The grayscale you have seen is ```Color``` feature. every features have to be located between ```CustomPostProcessInitializeFeature``` and ```CustomPostProcessFinalizeFeature```.

You can see it, the "Feature" of the color feature is 0. And the ```Applied Features``` flag in ```CustomPostProcessComponent``` contains 0 too.

![image](https://user-images.githubusercontent.com/22534449/131367173-651f42f6-35a5-446a-a093-18d7e202755f.png)
![image](https://user-images.githubusercontent.com/22534449/131367186-91f42bbb-5005-4518-9164-51634a00d261.png)

As you may have noticed, the Component and Adapter can apply features to the Source by ```Applied Features``` flag of themselves if it contains any feature added to ```ForwardRendererData```.

...and ```CustomPostProcessViaScript``` is exist also, please see that if you want to do via script.