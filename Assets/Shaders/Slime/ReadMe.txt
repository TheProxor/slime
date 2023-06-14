В шейдере Slime после перегенрации через Amplify нужно в Forward Pass заменить один инклюд:

Это
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

Заменить на это
#include "CustomLighting.hlsl"