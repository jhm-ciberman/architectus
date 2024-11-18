using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using LifeSim.Support.Numerics;

namespace Architectus;

public class HouseGenerator
{
    private readonly Random _random;

    public Vector2Int PlotSize { get; set; } = new Vector2Int(10, 10);

    public HouseGenerator(Random? random = null)
    {
        this._random = random ?? Random.Shared;
    }

    public bool TryGenerate([NotNullWhen(true)] out HouseLot? house)
    {
        //var template = new TwoRoomHouseTemplate();
        var templates = new HouseTemplate[]
        {
            new TwoRoomHouseTemplate(),
            new SmallFamiliarHouseTemplate() { NumberOfBedrooms = 2 },
            new SmallFamiliarHouseTemplate() { NumberOfBedrooms = 3 },
            new SmallFamiliarHouseTemplate() { NumberOfBedrooms = 4 },
        };

        List<HouseLot> generated = new();
        foreach (var template in templates)
        {
            if (template.TryBuild(this.PlotSize, this._random, out var houseLot))
            {
                generated.Add(houseLot);
            }
        }

        Console.WriteLine($"Generated {generated.Count} houses.");

        if (generated.Count == 0)
        {
            house = null;
            return false;
        }

        house = generated[this._random.Next(generated.Count)];
        return true;
    }
}
