using AutoFixture;
using ChanceNET;
using NUnit.Framework;

namespace Tests.EditModeTests
{
    public class MeshDataTests
    {
        private readonly Chance _chance;
        private readonly Fixture _fixture;
        public MeshDataTests()
        {
            _chance = new Chance();
            _fixture = new Fixture();
        }
        
        [Test]
        public void GivenTriangleVertices_WhenCallingAddTriangle_AddTriangle()
        {
            var sut = new MeshData(10, 10);
            var a = _chance.Integer();
            var b = _chance.Integer();
            var c = _chance.Integer();
            
            sut.AddTriangle(a, b, c);
            
            Assert.AreEqual(a, sut.Triangles[0]);
            Assert.AreEqual(b, sut.Triangles[1]);
            Assert.AreEqual(c, sut.Triangles[2]);
        }
        
        [Test]
        public void GivenTriangleVertices_WhenCallingAddTriangleMultipleTimes_AddTriangles()
        {
            var sut = new MeshData(10, 10);
            var a = _chance.Integer();
            var b = _chance.Integer();
            var c = _chance.Integer();
            var a2 = _chance.Integer();
            var b2 = _chance.Integer();
            var c2 = _chance.Integer();
            var a3 = _chance.Integer();
            var b3 = _chance.Integer();
            var c3 = _chance.Integer();
            
            sut.AddTriangle(a, b, c);
            sut.AddTriangle(a2, b2, c2);
            sut.AddTriangle(a3, b3, c3);
            
            Assert.AreEqual(a, sut.Triangles[0]);
            Assert.AreEqual(b, sut.Triangles[1]);
            Assert.AreEqual(c, sut.Triangles[2]); 
            Assert.AreEqual(a2, sut.Triangles[3]);
            Assert.AreEqual(b2, sut.Triangles[4]);
            Assert.AreEqual(c2, sut.Triangles[5]); 
            Assert.AreEqual(a3, sut.Triangles[6]);
            Assert.AreEqual(b3, sut.Triangles[7]);
            Assert.AreEqual(c3, sut.Triangles[8]);
        }

        [Test]
        public void GivenMeshData_WhenCallingCreateMesh_ThenCreateMesh()
        {
            var sut = _fixture.Create<MeshData>();
            var result = sut.CreateMesh();
            
            Assert.AreEqual(sut.Triangles, result.triangles);
            Assert.AreEqual(sut.Uvs, result.uv);
            Assert.AreEqual(sut.Vertices, result.vertices);
        }
    }
}