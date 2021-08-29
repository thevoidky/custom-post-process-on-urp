# Custom Post-process on URP

Support shader-based customized post-process features on Universal Render Pipeline.

## Requirement

- UniTask (https://github.com/Cysharp/UniTask)
- Tested on Unity 2020.3.6f1

## Installation

It is supported via Package Manager with git URL only.

![01](https://user-images.githubusercontent.com/22534449/131255940-c0896eb0-5a76-4d7e-93f2-77d83841a900.png)

Input ```https://github.com/thevoidky/custom-post-process-on-urp.git#experimental``` and ```Add```.

![02](https://user-images.githubusercontent.com/22534449/131256532-fe0212f2-4d5b-48d5-b66b-5bc55d227ab5.png)

## Sample

![03](https://user-images.githubusercontent.com/22534449/131256592-8ed9da86-f2bf-4bea-891a-9d06f4e13881.png)

If you succeeded to install the package, you could see the sample,

![04](https://user-images.githubusercontent.com/22534449/131256700-f835ce5a-c0c7-4837-ba6a-3b1225270667.png)

and did import, you could see sample scene. This sample is preset of an URP project that is applied custom post-process.

![image](https://user-images.githubusercontent.com/22534449/131256909-c7a83919-60dd-4b53-85c3-9b87527fcc83.png)
![image](https://user-images.githubusercontent.com/22534449/131256927-4900fcfa-b599-49ee-9f7a-aaece2289965.png)

You could see a CustomPostProcessSource component at "Main Camera" GameObject and a CustomPostProcessAdapter component at "CustomPostProcess" GameObject.

Let's play the scene and click "Toggle to on" of the CustomPostProcessAdapter.
