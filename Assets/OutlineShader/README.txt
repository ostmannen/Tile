När du applicerar de nya materialen till prefabs
    Rör inte TopRenderMat om du inte vet exakt vad du ska göra.
    Den fungerar som en specialiserad depth mask

    OutlineMaterial kan du dra i inställningarna till. Blå färg funkar inte for some reason dock.
    OutlineHeightOffset kommer anpassas av Thickness, så den behöver du inte röra.

    På din prefab ska du skapa 2 nya index i Mesh Rendererns material.
    element 0 - ditt base material
    element 1 - TopRenderMat
    element 2 - OutlineMaterial
        Ordningen är viktig för den är hårdkodad, du kan ändra det om det är störigt
    Anledningen till att vi inte har referenser till materialen, utan istället hämtar dem från
    Mesh Renderer är att vi bara vill påverka den specifika instansen av materialet.