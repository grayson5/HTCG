using UnityEngine;
using System.Collections;

public interface ICharacter: IIdentifiable
{	
    int Health { get;    set;}
    Player Owner();
    void Die();
}

public interface IIdentifiable
{
    int ID { get; }
}
