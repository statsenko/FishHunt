using UnityEngine;
using System.Collections;
using System;
namespace Extensions 
{
	public static class UIWidgetScaleExtensions
	{
		/*
		public static Vector3 CalculateFitSize(this UIWidget value, Vector2 maxSize_)
		{
			UIWidget widget_ = value;
			//temporary save current  size
			Vector3 currentSize_ = widget_.transform.localScale;
		
			//get original  size
			widget_.MakePixelPerfect();
			Vector3 originalSize_ = widget_.transform.localScale;

			Vector3 fitScale_ = CalculateFitScale(widget_, maxSize_);
			Vector3 newSize_ = new Vector3(originalSize_.x*fitScale_.x, originalSize_.y*fitScale_.y, 1.0f);
			
			//restore  size
			widget_.transform.localScale = currentSize_;
			return newSize_;
		}
		
		public static Vector3 CalculateFitScale(this UIWidget value, Vector2 maxSize_)
		{
			UIWidget widget_ = value;
			
			Vector2 fillScale_ = CalculateFillScale(widget_, maxSize_);
			
			Vector2 fitScale_ = Vector2.one * Mathf.Clamp01(Mathf.Min(fillScale_.x, fillScale_.y));
			
			return new Vector3( fitScale_.x, fitScale_.y, 1f);
		}
		
		public static Vector3 CalculateFillScale(this UIWidget value, Vector2 maxSize_)
		{
			UIWidget widget_ = value;
			Vector3 originalSize_ = widget_.transform.localScale;
			
			Vector2 fillScale_ = Vector3.zero;
			if (originalSize_.x != 0f && maxSize_.x != 0f)
				fillScale_.x = maxSize_.x/originalSize_.x;
			if (originalSize_.y != 0f && maxSize_.y != 0f)
				fillScale_.y = maxSize_.y/originalSize_.y;
			
			return new Vector3( fillScale_.x, fillScale_.y, 1.0f);
		}
		
		public static  void ScaleToFill(this UIWidget value, Vector2 newSize_)
		{
			UIWidget widget_ = value;
			widget_.transform.localScale = new Vector3(newSize_.x, newSize_.y, 1.0f);
		}
		
		public static  void ScaleToFit(this UIWidget value, Vector2 newSize_)
		{
			UIWidget widget_ = value;
			ScaleToFill(widget_, CalculateFitSize(widget_, newSize_));
		}

		public static void ScaleToFit(this UIWidget value)
		{
			UIWidget widget_ = value;
			ScaleToFit(widget_, widget_.transform.localScale);
		}

		public static void ScaleToFill(this UIWidget value)
		{
			UIWidget widget_ = value;
			ScaleToFill(widget_, widget_.transform.localScale);
		}
		*/
	}
}