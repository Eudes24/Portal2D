using Godot;
using System;

public partial class NiveauSuivant : Area2D
{
	public static int NumeroNiveauSuivant = 2;
	public string PorteNiveauSuivant = $"res://Niveaux/Niveau{NumeroNiveauSuivant}.tscn";

	public NiveauSuivant()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

	public void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			GD.Print("Bravo tu as fini le niveau");
			NumeroNiveauSuivant++;
			
			// On diffère le changement de scène pour éviter les suppressions pendant l'exécution (warning du moteur de jeu)
			CallDeferred(nameof(ChangerDeScene));
		}
	}

	// On change de scene
	public void ChangerDeScene()
	{
		GetTree().ChangeSceneToFile(PorteNiveauSuivant);
	}
}
