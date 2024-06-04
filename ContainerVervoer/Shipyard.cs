﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerVervoer;
public class Shipyard
{
    public static List<Row> LoadShip(Ship ship, List<Container> containers)
    {
        // Maak een lijst van rijen voor het schip
        List<Row> rows = new List<Row>();
        for (int i = 0; i < ship.Length; i++)
        {
            rows.Add(new Row());
        }

        // Maak stacks aan voor elke rij op het schip
        foreach (var row in rows)
        {
            for (int i = 0; i < ship.Width; i++)
            {
                row.Stacks.Add(new Stack());
            }
        }

        // Houd bij welke containers al zijn geplaatst
        HashSet<Container> placedContainers = new HashSet<Container>();

        // Plaats containers van het type "ValuableCooled" op rij 1
        foreach (var container in containers.Where(c => c.ContainerType == ContainerType.ValuableCooled))
        {
            bool placed = false;
            int valuedCooledCount = rows[0].Stacks.SelectMany(s => s.Containers).Count(c => c.ContainerType == ContainerType.ValuableCooled);
            if (valuedCooledCount < ship.Width)
            {
                // Plaats de container in rij 1, op de stack met het minste gewicht
                var minWeightStack = rows[0].Stacks.OrderBy(s => s.GetCurrentWeight()).First();
                if (minWeightStack.GetCurrentWeight() + container.Weight <= Stack.MaxWeightPerStack)
                {
                    minWeightStack.Containers.Add(container);
                    placedContainers.Add(container);
                    placed = true;
                }
            }
            if (!placed)
            {
                Console.WriteLine($"Waarschuwing: Container van type {container.ContainerType} ({container.Weight} kg) kon niet worden geplaatst op rij 1. Maximaal toegestane aantal containers van dit type bereikt.");
            }
        }

        // Plaats containers van het type "Cooled" op rij 1
        foreach (var container in containers.Where(c => c.ContainerType == ContainerType.Coolable))
        {
            bool placed = false;
            var minWeightStack = rows[0].Stacks.OrderBy(s => s.GetCurrentWeight()).First();
            if (minWeightStack.GetCurrentWeight() + container.Weight <= Stack.MaxWeightPerStack)
            {
                minWeightStack.Containers.Add(container);
                placedContainers.Add(container);
                placed = true;
            }
            if (!placed)
            {
                Console.WriteLine($"Waarschuwing: Container van type {container.ContainerType} ({container.Weight} kg) kon niet worden geplaatst op rij 1.");
            }
        }

        // Verdeel de "Valuable" containers over rijen vanaf rij 2
        var valuableContainers = containers.Where(c => c.ContainerType == ContainerType.Valuable).OrderByDescending(c => c.Weight).ToList();
        foreach (var container in valuableContainers)
        {
            bool placed = false;
            for (int i = 1; i < rows.Count; i++)
            {
                foreach (var stack in rows[i].Stacks.OrderBy(s => s.GetCurrentWeight()))
                {
                    // Controleer of er al een "Valuable" container op deze stack staat
                    if (stack.Containers.Any(c => c.ContainerType == ContainerType.Valuable))
                    {
                        continue;
                    }
                    if (stack.GetCurrentWeight() + container.Weight <= Stack.MaxWeightPerStack)
                    {
                        stack.Containers.Add(container);
                        placedContainers.Add(container);
                        placed = true;
                        break;
                    }
                }
                if (placed) break;
            }
            if (!placed)
            {
                Console.WriteLine($"Waarschuwing: Container van type {container.ContainerType} ({container.Weight} kg) kon niet worden geplaatst.");
            }
        }

        // Verdeel de "Normal" containers over de rijen, beginnend vanaf rij 2
        var normalContainers = containers.Where(c => c.ContainerType == ContainerType.Normal).OrderByDescending(c => c.Weight).ToList();
        foreach (var container in normalContainers)
        {
            bool placed = false;
            for (int i = 1; i < rows.Count; i++)
            {
                // Zoek de stack met het minste gewicht in de huidige rij
                var minWeightStack = rows[i].Stacks.OrderBy(s => s.GetCurrentWeight()).First();
                // Controleer of het gewicht van de stack het maximale gewicht niet overschrijdt
                if (minWeightStack.GetCurrentWeight() + container.Weight <= Stack.MaxWeightPerStack)
                {
                    minWeightStack.Containers.Add(container);
                    placedContainers.Add(container);
                    placed = true;
                    break;
                }
            }
            if (!placed)
            {
                Console.WriteLine($"Waarschuwing: Container van type {container.ContainerType} ({container.Weight} kg) kon niet worden geplaatst.");
            }
        }

        return rows;
    }
}





