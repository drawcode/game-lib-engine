using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIToolkit {

public class UIVerticalLayout : UIAbstractContainer
{	
	public UIVerticalLayout( int spacing ) : base( UILayoutType.Vertical )
	{
		_spacing = spacing;
	}
}
	
}