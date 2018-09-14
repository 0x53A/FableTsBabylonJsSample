import * as React from 'react';
import * as BABYLON from 'babylonjs';

import BabylonScene from './SceneComponent';


export type SingleFileBabylonViewerProps = {
    stlUrl: string,
    stlFileName: string
};

export default class SingleFileBabylonViewer extends React.Component<SingleFileBabylonViewerProps, {}> {

    private scene: BABYLON.Scene;
    private camera: BABYLON.ArcRotateCamera;
    private currentMeshes: BABYLON.AbstractMesh[] | null;

    public componentWillReceiveProps(p: SingleFileBabylonViewerProps) {
        if (p.stlUrl != this.props.stlUrl || p.stlFileName != this.props.stlFileName) {
            if (this.currentMeshes != null) {
                for (var mesh of this.currentMeshes) {
                    mesh.dispose();
                }
                this.currentMeshes = null;
            }
            
            console.log("loading from url: " + p.stlUrl + " ;filename: " +p.stlFileName);
            const self = this;
            BABYLON.SceneLoader.ImportMesh("", p.stlUrl, p.stlFileName, this.scene, function (newMeshes) {
                // remember the meshes
                self.currentMeshes = newMeshes;
            });
            console.log("loaded from url: " + p.stlUrl + " ;filename: " + p.stlFileName)
        }
    }

    public onSceneMount = (e: any) => {
        const { canvas, scene, engine } = e;
        this.scene = scene;

        // This creates and positions a free camera (non-mesh)
        const camera = new BABYLON.ArcRotateCamera("camera1", 1.5, 1, 2, new BABYLON.Vector3(0, 5, -10), scene);

        // This targets the camera to scene origin
        camera.setTarget(BABYLON.Vector3.Zero());

        // This attaches the camera to the canvas
        camera.attachControl(canvas, false);

        // This creates a light, aiming 0,1,0 - to the sky (non-mesh)
        const light = new BABYLON.HemisphericLight("light1", new BABYLON.Vector3(0, 1, 0), scene);

        // Default intensity is 1. Let's dim the light a small amount
        light.intensity = 0.7;

        // Our built-in 'sphere' shape. Params: name, subdivs, size, scene
        //const sphere = BABYLON.Mesh.CreateSphere("sphere1", 16, 2, scene);

        //// Move the sphere upward 1/2 its height
        //sphere.position.y = 1;

        console.log("loading from url: " + this.props.stlUrl + " ;filename: " + this.props.stlFileName)
        const self = this;
        const x = BABYLON.SceneLoader.ImportMesh("", this.props.stlUrl, this.props.stlFileName, scene, function (newMeshes) {
            // remember the meshes
            self.currentMeshes = newMeshes;
        });
        console.log("loaded from url: " + this.props.stlUrl + " ;filename: " + this.props.stlFileName)

        // Our built-in 'ground' shape. Params: name, width, depth, subdivs, scene
        // var ground = BABYLON.Mesh.CreateGround("ground1", 6, 6, 2, scene);

        engine.runRenderLoop(() => {
            if (scene) {
                scene.render();
            }
        });
    }

    public render() {
        return (
            <BabylonScene onSceneMount={this.onSceneMount} width={1400} height={700} />
        )
    }
}



