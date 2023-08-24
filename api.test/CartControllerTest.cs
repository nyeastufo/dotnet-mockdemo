using api.Controllers;
using api.Models;
using api.Services;
using Moq;

namespace api.test;

public class Tests
{
    private CartController controller;
    private Mock<IPaymentService> paymentServiceMock;
    private Mock<ICartService> cartServiceMock;
    private Mock<IShipmentService> shipmentServiceMock;
    private Mock<ICard> cardMock;
    private Mock<IAddressInfo> addressInfoMock;
    private List<CartItem> items;

    [SetUp]
    public void Setup()
    {
        cartServiceMock = new Mock<ICartService>();
        paymentServiceMock = new Mock<IPaymentService>();
        shipmentServiceMock = new Mock<IShipmentService>();

        cardMock = new Mock<ICard>();
        addressInfoMock = new Mock<IAddressInfo>();

        var cartItemMock = new Mock<CartItem>();
        cartItemMock.Setup(item => item.Price).Returns(10);

        items = new List<CartItem>()
        {
            cartItemMock.Object
        };

        cartServiceMock.Setup(c => c.Items()).Returns(items.AsEnumerable());
        controller = new CartController(cartServiceMock.Object, paymentServiceMock.Object, shipmentServiceMock.Object);
    }

    [Test]
    public void ShouldReturnCharged()
    {
        // Arrange
        paymentServiceMock.Setup(p => p.Charge(It.IsAny<double>(), cardMock.Object)).Returns(true);

        // Act
        var result = controller.CheckOut(cardMock.Object, addressInfoMock.Object);

        // Assert
        shipmentServiceMock.Verify(s => s.Ship(addressInfoMock.Object, items.AsEnumerable()), Times.Once());

        Assert.That(result, Is.EqualTo("charged"));
    }

    [Test]
    public void ShouldReturnNotCharged()
    {
        // Arrange
        paymentServiceMock.Setup(p => p.Charge(It.IsAny<double>(), cardMock.Object)).Returns(false);

        // Act
        var result = controller.CheckOut(cardMock.Object, addressInfoMock.Object);

        // Assert
        shipmentServiceMock.Verify(s => s.Ship(addressInfoMock.Object, items.AsEnumerable()), Times.Never());
        Assert.That(result, Is.EqualTo("not charged"));
    }
}