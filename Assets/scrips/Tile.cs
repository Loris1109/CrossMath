using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class Tile : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private GameObject isSelected;
    [SerializeField] private GameObject equalSign;
    [SerializeField] private GameObject oneNum;
    [SerializeField] private GameObject twoNum1;
    [SerializeField] private GameObject twoNum2;
    private int displayedNum;
    [SerializeField] private Sprite[] numbers;
    [SerializeField] private Sprite[] arithmeticSymbols;
    [SerializeField] private EventTouch eventTouch;
    private bool istouched = false;
    private int gameHeight;
    private int gameWidth;
    private bool isSelected_bool = false;
    private bool toBeSolved = false;
    [SerializeField] private Color baseColor;
    [SerializeField] private Color correctColor;
    public int index;
    [SerializeField]private int storedValue;
    [SerializeField]private string storedValuePos;
    [SerializeField]private string collumOrRow;
   public void Initialise()
   {
      GetComponent<SpriteRenderer>().color = baseColor;
   }

  public Bounds colBounds()
  {
    return GetComponent<BoxCollider2D>().bounds;
  }

   public int getIndex()
   {
        return index;
   }
   public void setIndex(int newIndex)
   {
        index = newIndex;
   }

   public void selectTile(bool onAndOff)
   {
    isSelected.SetActive(onAndOff);
      switch(storedValuePos)
      {
        case "+":
          oneNum.GetComponent<SpriteRenderer>().sprite = arithmeticSymbols[0];
          oneNum.SetActive(true);
          break;
        case "-":
          oneNum.GetComponent<SpriteRenderer>().sprite = arithmeticSymbols[1];
          oneNum.SetActive(true);
          break;
        case "*":
          oneNum.GetComponent<SpriteRenderer>().sprite = arithmeticSymbols[2];
          oneNum.SetActive(true);
          break;
        case "/":
          oneNum.GetComponent<SpriteRenderer>().sprite = arithmeticSymbols[3];
          oneNum.SetActive(true);
          break;
        case "=":
          oneNum.GetComponent<SpriteRenderer>().sprite = arithmeticSymbols[4];
          oneNum.SetActive(true);
          break;
        default:
          displayNumbers(storedValue);
          break;
      }
      isSelected_bool = true;
   }

   private int getFirstInt(int x)
   {
      x = Mathf.Abs(x);
      while(x > 9)
      {
        x /= 10;
      }
      return x;
   }
   
   public bool checkToBeSolved()
   {
      return toBeSolved;
   } 

   public void setToBeSolved(bool toBeSolved)
   {
      this.toBeSolved = toBeSolved;
      if(toBeSolved == true)
      {
        oneNum.SetActive(false);
        twoNum1.SetActive(false);
        twoNum2.SetActive(false);
        isSelected.GetComponent<SpriteRenderer>().color = new Color(114,202,92);
        displayedNum = -100;//imposible num to reach at the moment
        eventTouch.enabled = true;
      }
   }

   public void displayNumbers(int num)
   {
    if(istouched == true && displayedNum != -100)
    {
      if (displayedNum >= 10)
      {
        oneNum.GetComponent<SpriteRenderer>().sprite = numbers[num];
        twoNum1.SetActive(false);
        twoNum2.SetActive(false);
        oneNum.SetActive(true);
        displayedNum = num;
      }
      else
      {
        twoNum1.GetComponent<SpriteRenderer>().sprite = numbers[displayedNum];
        twoNum2.GetComponent<SpriteRenderer>().sprite = numbers[num];
        oneNum.SetActive(false);
        twoNum1.SetActive(true);
        twoNum2.SetActive(true);
        displayedNum = int.Parse(displayedNum.ToString() + num.ToString());
      }
      return;
    }
    else if(istouched == true && displayedNum == -100)
    {
      twoNum1.SetActive(false);
      twoNum2.SetActive(false);
      oneNum.SetActive(false);
    }
    if(Math.Abs(num) >= 10)
    {
      int num1 = getFirstInt(num);
      twoNum1.GetComponent<SpriteRenderer>().sprite = numbers[num1];
      twoNum2.GetComponent<SpriteRenderer>().sprite = numbers[Math.Abs(num)-num1*10];
      if(num < 0)
      {
        twoNum1.transform.GetChild(0).gameObject.SetActive(true);  
      }
      twoNum1.SetActive(true);
      twoNum2.SetActive(true);
    }
    else
    {
      oneNum.GetComponent<SpriteRenderer>().sprite = numbers[Math.Abs(num)];
      if(num < 0)
      {
        twoNum1.transform.GetChild(0).gameObject.SetActive(true);  
      }
      oneNum.SetActive(true);
    }
    displayedNum = num;
   }

   public void setTouched(bool istouched)
   {
      this.istouched = istouched;
      if(istouched == false)
      {
        if(displayedNum == storedValue)
        {
          gameManager.solvedEquation();
          isSelected.GetComponent<SpriteRenderer>().color = correctColor;
        }
        setToBeSolved(false);
        displayedNum = -100;
      }
   }

   public void setGameManager(GameManager manager)
   {
    this.gameManager = manager;
   }
   public bool checkSelected()
   {
      return isSelected_bool;
   }
   public int getXValue()
   {
     return (int)transform.position.x;
   }
   public int getYValue()
   {
     return (int)transform.position.y;
   }

   public int getGameHight()
   {
      return gameHeight;
   }
   public void setGameHight(int gameHeight)
   {
    this.gameHeight = gameHeight;
   }

   public int getGameWidth()
   {
     return gameWidth;
   }

   public void setGameWidth(int gameWidth)
   {
      this.gameWidth = gameWidth;
   }

   public int getStoredValue()
   {
     return storedValue;
   }
   public void setStoredValue(int value)
   {
     storedValue = value;
   }

    public string getStoredValuePos()
   {
     return storedValuePos;
   }
   public void setStoredValuePos(string pos)
   {  
    storedValuePos = pos;
   }

   public string getCollumOrRow()
   {
      return collumOrRow;
   }

   public void setCollumOrRow(string collumOrRow)
   {
      if(collumOrRow == "collum")
      {
        this.collumOrRow = "collum";
      }
      else if(collumOrRow == "row")
      {
        this.collumOrRow = "row";
      }
      else if(collumOrRow == "both")
      {
        this.collumOrRow = "both";
      }
   }

}
