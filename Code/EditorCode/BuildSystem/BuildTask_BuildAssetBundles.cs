﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace UnityTK.BuildSystem
{
	/// <summary>
	/// <see cref="BuildTask"/> which will build all asset bundles specified in the project.
	/// </summary>
	[CreateAssetMenu(fileName = "BuildAssetBundlesTask", menuName = "UnityTK/BuildSystem/Build AssetBundles Task")]
	public class BuildTask_BuildAssetBundles : BuildTask
	{
		[Header("Task")]
		public string subfolder;

		[Header("Build config")]
		public BuildAssetBundleOptions[] bundleOptions;
		public BuildTarget buildTarget;

		public override void Run(BuildJob job, BuildJobParameters parameters)
		{
			BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
			for (int i = 0; i < this.bundleOptions.Length; i++)
				options |= this.bundleOptions[i];

			// Create directory if not existing
			string path = Path.Combine(parameters.destination, this.subfolder);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			BuildPipeline.BuildAssetBundles(path, options, this.buildTarget);
		}
	}
}