using System;
using System.Collections.Generic;
using System.Linq;

public class OrganismSpeciesManager
{  
    public Dictionary<OrganismSpecies, List<Organism>> organismSpeciesCollection;
    public static OrganismSpeciesManager Instance { get; private set; }

    // Generate a new list of species and default traits.
    public void InitializeSpecies(int speciesCount)
    {
        organismSpeciesCollection = new Dictionary<OrganismSpecies, List<Organism>>();
        for(int i = 0; i<speciesCount;i++)
        {
            var species = new OrganismSpecies(TraitsManager.GenerateParentTraits(null));
            organismSpeciesCollection.Add(species, new List<Organism>());
        }
    }

    public void AssignOrganismToSpecies(Organism org)
    {
        if(organismSpeciesCollection == null)
            organismSpeciesCollection = new Dictionary<OrganismSpecies, List<Organism>>();

         // Assign this organism to a species with the lowest organism count to keep it even
        var keyWithLowestOrganisms = organismSpeciesCollection.OrderBy(kvp => kvp.Value.Count).FirstOrDefault();
        if(keyWithLowestOrganisms.Key != null)
        {
            organismSpeciesCollection[keyWithLowestOrganisms.Key].Add(org);
        }
    }

}


