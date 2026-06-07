export var Microsoft;
(function (Microsoft) {
    var FluentUI;
    (function (FluentUI) {
        var Blazor;
        (function (Blazor) {
            var Overflow;
            (function (Overflow) {
                let resizeObserver;
                let observerAddRemove;
                let lastHandledState = { id: null, isHorizontal: null };
                function Initialize(dotNetHelper, id, isHorizontal, querySelector, threshold) {
                    let localSelector = querySelector;
                    if (!localSelector) {
                        localSelector = ".fluent-overflow-item";
                    }
                    observerAddRemove = new MutationObserver(mutations => {
                        mutations.forEach(mutation => {
                            if (mutation.type !== 'childList' && (mutation.addedNodes.length > 0 || mutation.removedNodes.length > 0)) {
                                return;
                            }
                            const node = mutation.addedNodes.length > 0 ? mutation.addedNodes[0] : mutation.removedNodes[0];
                            if (node.nodeType !== Node.ELEMENT_NODE || !node.matches(localSelector)) {
                                return;
                            }
                            Refresh(dotNetHelper, id, isHorizontal, querySelector, threshold);
                        });
                    });
                    const el = document.getElementById(id);
                    if (resizeObserver && el) {
                        resizeObserver.unobserve(el);
                    }
                    let resizeTimeout;
                    resizeObserver = new ResizeObserver(() => {
                        clearTimeout(resizeTimeout);
                        resizeTimeout = window.setTimeout(() => {
                            Refresh(dotNetHelper, id, isHorizontal, querySelector, threshold);
                        }, 100);
                    });
                    if (el) {
                        resizeObserver.observe(el);
                        observerAddRemove.observe(el, { childList: true, subtree: false });
                    }
                    lastHandledState.id = id;
                    lastHandledState.isHorizontal = isHorizontal;
                }
                Overflow.Initialize = Initialize;
                function Refresh(dotNetHelper, id, isHorizontal, querySelector, threshold) {
                    const container = document.getElementById(id);
                    if (!container)
                        return;
                    let localQuerySelector;
                    if (!querySelector) {
                        localQuerySelector = ":scope .fluent-overflow-item";
                    }
                    else {
                        localQuerySelector = ":scope >" + querySelector;
                    }
                    const allItems = container.querySelectorAll(localQuerySelector);
                    const items = container.querySelectorAll(localQuerySelector + ":not([fixed])");
                    const fixedItemsFromSelector = container.querySelectorAll(localQuerySelector + "[fixed]");
                    const otherFixedItems = container.querySelectorAll(":scope > [fixed]:not(" + localQuerySelector + ")");
                    const fixedItems = [
                        ...Array.from(fixedItemsFromSelector),
                        ...Array.from(otherFixedItems)
                    ].filter(el => el.getAttribute("fixed") !== "ellipsis");
                    const ellipsisItems = Array.from(container.querySelectorAll(localQuerySelector + "[fixed='ellipsis']"));
                    let ellipsisTotal = 0;
                    let containerGap = parseFloat(window.getComputedStyle(container).gap);
                    if (!containerGap)
                        containerGap = 0;
                    ellipsisItems.forEach((el, idx) => {
                        el.overflowSize = isHorizontal ? getElementWidth(el) : getElementHeight(el);
                        ellipsisTotal += el.overflowSize || 0;
                        if (idx > 0)
                            ellipsisTotal += containerGap;
                    });
                    let itemsTotalSize = threshold > 0 ? 10 : 0;
                    let containerMaxSize = isHorizontal ? container.offsetWidth : container.offsetHeight;
                    let overflowChanged = false;
                    containerMaxSize -= threshold;
                    const availableSize = containerMaxSize - fixedItems.reduce((sum, el, idx) => sum + (el.overflowSize || 0) + (idx > 0 ? containerGap : 0), 0);
                    if (ellipsisTotal > availableSize) {
                        ellipsisItems.forEach(el => {
                            el.style.flexShrink = "1";
                        });
                    }
                    else {
                        ellipsisItems.forEach(el => {
                            el.style.flexShrink = "0";
                        });
                    }
                    if (lastHandledState.id === id && lastHandledState.isHorizontal !== isHorizontal) {
                        allItems.forEach(element => {
                            element.removeAttribute("overflow");
                            element.overflowSize = null;
                        });
                    }
                    fixedItems.forEach(element => {
                        element.overflowSize = isHorizontal ? getElementWidth(element) : getElementHeight(element);
                        element.overflowSize = (element.overflowSize || 0) + containerGap;
                        itemsTotalSize += element.overflowSize;
                    });
                    items.forEach(element => {
                        const isOverflow = element.hasAttribute("overflow");
                        if (!isOverflow) {
                            element.overflowSize = isHorizontal ? getElementWidth(element) : getElementHeight(element);
                            element.overflowSize = (element.overflowSize || 0) + containerGap;
                        }
                        itemsTotalSize += element.overflowSize || 0;
                        if (containerMaxSize > 0) {
                            if (itemsTotalSize > containerMaxSize) {
                                if (!isOverflow) {
                                    element.setAttribute("overflow", "");
                                    overflowChanged = true;
                                }
                            }
                            else {
                                if (isOverflow) {
                                    element.removeAttribute("overflow");
                                    overflowChanged = true;
                                }
                            }
                        }
                    });
                    if (overflowChanged) {
                        const listOfOverflow = [];
                        items.forEach(element => {
                            listOfOverflow.push({
                                Id: element.id,
                                Overflow: element.hasAttribute("overflow"),
                                Text: element.innerText.trim()
                            });
                        });
                        dotNetHelper.invokeMethodAsync("OverflowRaisedAsync", listOfOverflow);
                    }
                    lastHandledState.id = id;
                    lastHandledState.isHorizontal = isHorizontal;
                }
                Overflow.Refresh = Refresh;
                function Dispose(id) {
                    const el = document.getElementById(id);
                    if (el) {
                        resizeObserver?.unobserve(el);
                        observerAddRemove?.disconnect();
                    }
                }
                Overflow.Dispose = Dispose;
                function getElementWidth(element) {
                    const style = window.getComputedStyle(element);
                    const width = element.offsetWidth;
                    const margin = parseFloat(style.marginLeft) + parseFloat(style.marginRight);
                    return width + margin;
                }
                function getElementHeight(element) {
                    const style = window.getComputedStyle(element);
                    const height = element.offsetHeight;
                    const margin = parseFloat(style.marginTop) + parseFloat(style.marginBottom);
                    return height + margin;
                }
            })(Overflow = Blazor.Overflow || (Blazor.Overflow = {}));
        })(Blazor = FluentUI.Blazor || (FluentUI.Blazor = {}));
    })(FluentUI = Microsoft.FluentUI || (Microsoft.FluentUI = {}));
})(Microsoft || (Microsoft = {}));
